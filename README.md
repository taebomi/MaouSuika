# 마왕 수박 게임 (Maou Suika)
> 써드파티 에셋 라이선스로 인해 전체 프로젝트가 아닌 C# 스크립트만 포함

> 퍼즐 (수박 게임) | Unity, C# | 1인 개발 | Android | **출시** (Google Play, 서비스 종료)
> 약 500 다운로드 · 평점 4.8 (평가 14 · 리뷰 6) · 개발 기간 2023.12–2024.05 (v1) · 2025.01–2025.07 (v2)

> 역중력 발사로 캡슐을 합치는 수박 게임

<img src="https://github.com/user-attachments/assets/7063f146-6f53-4cd6-a65d-2322dbcd25a8" width="560" />

### 트레일러
🔗 [트레일러](https://www.youtube.com/watch?v=L1Ubq69U_OY)

---

## 기술 스택

- **Unity, C#**
- **FMOD** — 음악 타이밍에 맞춘 연출 트리거 · 로우패스 파라미터를 게임 상태와 동기화
- **Google AdMob** — 전면 · 보상형 광고
- **Unity Gaming Services (UGS)** — 인앱 결제
- **GPGS** — 로그인 · 랭킹

---

## 핵심 1 — 아키텍처 설계: 무엇을 줄이고 무엇을 남길지

시스템이 커질수록 **강한 결합**으로 인한 문제(수정 시 연쇄 영향, 버그 추적 난항)를 반복해 겪었습니다. 그래서 결합도를 낮추는 방향으로 **마왕 수박 v1 → 텐 메이커 → 마왕 수박 v2** 세 프로젝트에 걸쳐 구조를 다듬었습니다.

### 1) 통신 — 객체를 어떻게 연결할 것인가

**첫 시도 — SO 이벤트 채널 전면 도입 (v1)** · [ScoreManager.cs](v1/Stage/Score/ScoreManager.cs)
Unity 공식 오픈 프로젝트와 SOAP(ScriptableObject Architecture Pattern)를 참고해 시스템 전반을 이벤트 채널로 연결했습니다.

<img src="https://github.com/user-attachments/assets/469f19d2-0a0f-47d8-a732-cf510da73c5f" width="560" />

```csharp
// ScoreManager.cs (v1) — 클래스의 거의 모든 멤버가 SO. 직접 참조 없이 SO로만 통신
[SerializeField] private IntEventSO       capsuleMergedEventSO;   // 합체 이벤트 (구독)
[SerializeField] private IntEventSO       scoreGetEventSO;        // 점수 획득 이벤트 (발행)
[SerializeField] private ObscuredIntVarSO curScore;               // 점수 '변수'까지 SO
// … highScoreVarSO · gashaponScoreDataSO · scoreMultiplierDataSO 등 전부 SO

private void Awake()
{
    capsuleMergedEventSO.OnEventRaised += OnCapsuleMerged;         // 직접 참조 대신 SO 구독
    curComboVarSO.OnValueChanged       += OnComboChanged;
}
```

초기엔 수정 영향이 격리되고 테스트도 쉬웠지만, 시스템이 커지자 한계가 드러났습니다.
- **암묵적 의존성** — 직접 참조만 없을 뿐 의존성은 그대로라 영향 범위 추적 불가
- **구독 타이밍 문제** — 순서가 중요한 종속 관계에서 실행 순서 보장 불가
- **관리 비용 급증** — `IntEventSO` 같은 SO 인스턴스가 과도하게 늘어 파일·참조 관리 부담

**두 번째 시도 — 서비스 로케이터 (텐 메이커)** · 전역 시스템에 한정해 싱글톤을 대체했으나, 호출 계층만 한 단계 늘었을 뿐 **구현체를 교체할 일이 없어** 무의미한 간접 계층으로 전락했습니다.

**정리 — 추상화 ≠ 간접 계층** · 다형성을 실제로 쓰지 않는 곳에 추상화를 덧씌우면 비용만 남는 간접 계층이 된다는 것을 체감했습니다.

### 2) 조립 — 의존성을 어떻게 주입할 것인가

[PuzzleController.cs](v2/Gameplay/Puzzle/PuzzleController.cs)가 하위 시스템 의존성을 **한곳에서 코드로 명시적으로 주입**합니다. 의존 관계가 코드에 드러나 앞서 겪은 이벤트 채널의 타이밍 문제를 해결했습니다. 다만 모두 코드로 주입하면 보일러플레이트가 비대해지므로, 주입 방식을 **혼용**했습니다 — 1:N 알림(게임 오버)은 이벤트 채널, 강하게 결합된 하위 시스템 참조는 인스펙터 직렬화.

```csharp
// PuzzleController.cs (v2) — 하위 시스템 의존성을 한곳에서 '코드로' 주입
public void Initialize(PlayerContext playerContext)
{
    _context = CreateContext(playerContext);                       // Builder로 컨텍스트 조립
    var inputController = InputManager.Instance.GetInputController(playerContext.PlayerIndex);

    suikaSystem.Initialize(mergeSystem, _context.TierDataTable);
    shooterSystem.Initialize(suikaSystem, queueSystem, inputController, area);
    scoreSystem.Initialize(_context.TierDataTable);
    gameOverSystem.Initialize(_context.PlayerContext.PlayerIndex, shooterSystem);

    RegisterEvents();                                              // 1:N 알림은 이벤트로 분리
}
```

### 3) 구조 — 기능을 어떻게 재사용 · 확장할 것인가

'댕댕이 서바이벌'에서 거대한 `EnemyBase`에 모든 적을 상속시켰다가 **불필요한 로직까지 강제 상속**되는 함정을 겪고, 변형의 성격에 따라 상속/컴포지션을 나눴습니다.

**상속 유지 (템플릿 메서드)** · [MergeEffectBase.cs](v2/Gameplay/Puzzle/Merge/Effect/MergeEffectBase.cs)
'등급별 머지 이펙트'는 런타임에 안 바뀌고 연출만 등급마다 다릅니다. 흐름은 Base가 쥐고 달라지는 연출만 추상 메서드로 위임 — 호출부는 하위 구현체를 모른 채 Base 타입으로 제어합니다.

```csharp
// MergeEffectBase.cs (v2) — 흐름은 Base 고정, '달라지는 연출'만 추상 메서드로 위임
public void Setup(MergeEffectColor color, float size)
{
    SetColor(color);                            // ↓ 하위 구현체가 채움
    SetSize(size);
    StartCoroutine(CheckIfAliveRoutine());      // 풀 반환까지 Base가 통제
}

protected abstract void SetSize(float size);
protected abstract void SetColor(MergeEffectColor color);
```

**컴포지션 채택 (전략 패턴)** · [IShooterInputStrategy.cs](v2/Gameplay/Puzzle/Shooter/Input/Strategy/IShooterInputStrategy.cs) · [ShooterInputModule.cs](v2/Gameplay/Puzzle/Shooter/Input/ShooterInputModule.cs)
입력 시스템은 런타임에 다양한 디바이스·입력 모드에 대응해야 합니다. 상속을 배제하고 입력 타입별 처리를 독립 전략으로 나눠, 모듈이 입력에 맞는 전략을 교체하도록 조립했습니다. 새 입력 방식은 **전략 클래스 하나만 추가**하면 되고, 각 전략은 결과 `ShooterInputCommand`만 반환해 *입력 → 명령 → 실행* 한 방향 흐름을 유지합니다.

```csharp
// ShooterInputModule.cs (v2) — 입력 타입별 전략을 Dictionary로 보유, 런타임에 교체
_strategies = new Dictionary<ShooterInputType, IShooterInputStrategy>
{
    { ShooterInputType.Drag,          new ShooterDragStrategy(area) },
    { ShooterInputType.Classic,       new ShooterClassicStrategy() },
    { ShooterInputType.Direct,        new ShooterDirectStrategy() },
    { ShooterInputType.VirtualCursor, new ShooterVirtualCursorStrategy() },
    // …
};

public ShooterInputCommand Process(ShooterInputResult input, float deltaTime)
{
    SwitchStrategy(input.InputType);                 // 입력에 맞는 전략으로 교체
    return _curStrategy.Process(input, deltaTime);   // 결과는 ShooterInputCommand 한 방향
}
```

### 결론 — 패턴의 적재적소

싱글톤 자체가 문제가 아니라 **'패턴의 적재적소'** 가 핵심이었습니다. 무조건 결합을 끊는 게 아니라, 무엇을 줄이고 무엇을 남길지의 기준을 세웠습니다.

| 설계 선택 | 적용 기준 | 적용 예시 |
|----------|----------|----------|
| 이벤트 채널 | 발신자가 수신자를 몰라도 되는 1:N 알림 | 게임 오버 이벤트 |
| 싱글톤 | 상시 존재하며 다형성이 불필요한 전역 매니저 | 인풋 매니저 |
| 직접 주입 | 결합이 강하고 변경이 드문 하위 시스템 조립 | 퍼즐 컨트롤러 |
| 상속 | 변형이 타입에 고정된 공통 동작 | 등급별 머지 이펙트 |
| 컴포지션 | 변형이 런타임에 바뀌는 처리 | 입력 시스템 |

---

## 핵심 2 — 반복 작업 자동화: 에디터 유틸리티 툴

AI를 보조 수단으로 활용해 툴 요구사항을 명세하고 코드를 빠르게 반복 개선하면서, 툴 제작 비용을 크게 낮췄습니다. 덕분에 반복 잦던 그래픽 에셋 파이프라인을 유틸리티 툴로 적극 대체했습니다.

### 스프라이트 슬라이서 · [SpriteRowSlicerWindow.cs](v2/TBM/Tools/Editor/SpriteRowSlicerWindow.cs)

<img src="https://github.com/user-attachments/assets/f9d71119-3077-45aa-9bfd-a8331f1aea72" width="560" />

유니티 기본 에디터가 지원하지 않는 **'행 단위 시트 선택적 슬라이스'** 툴.
- **기존 문제** — 타겟 행 선택 추출 불가, 단순 숫자 네이밍 탓에 수작업 분류 비효율
- **주요 기능** — 프리셋을 통한 행별 일괄 네이밍 + 타겟 행 추출
- **성과** — 객체당 5분+ → **30초 이내**

### 스프라이트 그룹 피벗 에디터 · [SpritePivotEditor.cs](v2/TBM/Tools/Editor/SpritePivotEditor.cs)

<img src="https://github.com/user-attachments/assets/ab1e9632-75e8-4dbf-a277-24efd81d8f04" width="560" />

추출한 행 단위 스프라이트의 피벗을 **그룹 단위로 일괄 설정**하는 툴.
- **기존 문제** — 스냅·그룹 설정 부재, 피벗 대신 Root Transform으로 우회
- **주요 기능** — 도트 그래픽에 맞춘 0.5칸 단위 스냅으로 그룹 피벗 일괄 설정
- **성과** — 우회용 Root Transform 제거 → 오브젝트 계층 **1단계 축소**

### 애니메이션 타일 생성기 · [AnimatedTileCreatorWindow.cs](v2/TBM/Tools/Editor/AnimatedTileCreatorWindow.cs)

<img src="https://github.com/user-attachments/assets/745e6ac3-532f-497c-a787-5058cc8127d9" width="560" />

스프라이트 시트에서 애니메이션 프레임을 선택해 타일을 생성하는 툴.
- **기존 문제** — 수많은 스프라이트 중 필요 프레임을 일일이 찾아 드래그하는 비효율
- **주요 기능** — 시트 내 타일을 클릭·드래그로 그룹화해 단일·다중 애니메이션 타일 생성
- **성과** — 이름 찾는 과정 없이 시각화된 프레임 클릭·드래그만으로 타일 생성

**결론** — 단순 반복에 낭비되던 시간을 핵심 로직 개발에 재투자하게 됐고, 무엇보다 *'수작업 vs 툴 자동화'* 를 합리적으로 판단하는 기준이 자리 잡았습니다.

---

## 핵심 코드 구조
<pre>
v2/  <i>(차기작 — 아키텍처 재정리 + 에디터 툴)</i>
├── <a href="v2/Gameplay/Puzzle">Gameplay/Puzzle/</a>
│   ├── <a href="v2/Gameplay/Puzzle/PuzzleController.cs">PuzzleController.cs</a>                 # 의존성 코드 주입 (조립) ★
│   ├── <a href="v2/Gameplay/Puzzle/Merge/Effect/MergeEffectBase.cs">Merge/Effect/MergeEffectBase.cs</a>    # 템플릿 메서드 (상속) ★
│   ├── Shooter/Input/
│   │   ├── <a href="v2/Gameplay/Puzzle/Shooter/Input/ShooterInputModule.cs">ShooterInputModule.cs</a>          # 전략 디스패치 (컴포지션) ★
│   │   └── <a href="v2/Gameplay/Puzzle/Shooter/Input/Strategy/IShooterInputStrategy.cs">Strategy/IShooterInputStrategy.cs</a>  # 전략 계약
│   └── <a href="v2/Gameplay/Puzzle/Score">Score/</a>                             # Model · View · System 분리
└── <a href="v2/TBM/Tools/Editor">TBM/Tools/Editor/</a>                      # 에디터 유틸리티 툴 ★
    ├── <a href="v2/TBM/Tools/Editor/SpriteRowSlicerWindow.cs">SpriteRowSlicerWindow.cs</a>           # 행 단위 슬라이서
    ├── <a href="v2/TBM/Tools/Editor/SpritePivotEditor.cs">SpritePivotEditor.cs</a>               # 그룹 피벗 에디터
    └── <a href="v2/TBM/Tools/Editor/AnimatedTileCreatorWindow.cs">AnimatedTileCreatorWindow.cs</a>       # 애니메이션 타일 생성기

v1/  <i>(출시작 — SO 이벤트 채널 전면 도입, 'Before' 비교용)</i>
├── <a href="v1/SO">SO/</a>                                    # ScriptableObject 이벤트·변수 채널
├── <a href="v1/Stage">Stage/</a>
│   └── <a href="v1/Stage/Score/ScoreManager.cs">Score/ScoreManager.cs</a>              # 모든 변수를 SO로 (통신 1차 시도) ★
└── <a href="v1/System">System/</a>                                # 공통 시스템 (Audio · UI · Scene 등)
</pre>
