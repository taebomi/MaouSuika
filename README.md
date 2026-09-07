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
- **R3 (v2)** — 발사·머지·게임 오버 결과 전달, 상태 구독 및 구독 수명 관리
- **Unity Input System (v2)** — 장치별 입력 처리와 퍼즐·스킬 액션 맵 전환
- **UniTask (v2)** — 씬 전환·게임 오버 연출의 비동기 흐름 처리
- **FMOD** — 음악 타이밍에 맞춘 연출 트리거 · 로우패스 파라미터를 게임 상태와 동기화
- **Google AdMob (v1)** — 전면 · 보상형 광고
- **Unity IAP · UGS (v1)** — 인앱 결제 및 서비스 초기화
- **GPGS (v1)** — 로그인 · 랭킹

---

## 핵심 1 — 아키텍처 설계: 의존성 관리의 시행착오를 통해 세운 설계 기준

기능을 추가하고 버그를 수정할 때 **한 기능의 변경이 여러 클래스에 연쇄적으로 영향을 미치는 문제**를 겪었습니다. 이를 개선하기 위해 마왕 수박 v1 → 텐 메이커 → 마왕 수박 v2에 걸쳐 구조를 다듬었고, **의존 관계와 책임의 소유자를 코드에 드러내는 것**을 설계 기준으로 삼았습니다.

### 1) 시행착오 — 직접 참조를 줄이자 의존성이 숨겨짐

**SO 이벤트 채널 전면 도입 (v1)** · [ScoreManager.cs](v1/Stage/Score/ScoreManager.cs)

Unity 공식 오픈 프로젝트와 SOAP(ScriptableObject Architecture Pattern)를 참고해 시스템 전반을 ScriptableObject 이벤트 채널로 연결했습니다. 점수 시스템에서도 합체 알림뿐 아니라 현재 점수·콤보·계산 데이터까지 SO로 참조했습니다.

<img src="https://github.com/user-attachments/assets/469f19d2-0a0f-47d8-a732-cf510da73c5f" width="560" />

직접 참조는 줄었지만, 프로젝트가 커지면서 다른 비용이 발생했습니다.

- **숨은 의존 관계** — 실제 연결이 에셋 뒤에 숨으면서 변경의 영향 범위를 추적하기 어려워졌습니다.
- **실행 순서 파악의 어려움** — 구독과 발행 시점에 따라 동작이 달라져, 초기화 순서가 중요한 관계를 관리하기 어려웠습니다.
- **에셋 관리 비용** — 이벤트·변수별 SO 인스턴스가 늘어 파일과 참조를 관리하는 부담이 커졌습니다.

**서비스 로케이터 적용 (텐 메이커)** — 이후 프로젝트에서는 전역 시스템에 한정해 싱글톤을 서비스 로케이터로 대체했습니다. 그러나 테스트 대역이나 플랫폼별 구현처럼 실제 구현체를 교체할 필요가 없어, 호출 계층만 늘어나고 의존성은 여전히 외부에서 드러나지 않았습니다.

두 경험을 통해 **참조를 없애는 것보다, 누가 무엇을 소유하고 어떤 순서로 연결하는지를 명확히 하는 것이 중요하다**고 판단했습니다.

### 2) 개선 — 책임에 따라 조립하고, 결과를 선택적으로 전달

v2에서는 게임의 상태와 생명주기를 책임 범위에 따라 계층화했습니다.

| 파일 | 책임 |
|------|------|
| [GameplayRoot.cs](v2/Assets/_Projects/Scripts/Gameplay/GameplayRoot.cs) | Loading·Playing·Paused·GameOver 상태, 초기화 및 상태별 갱신, 재시작 흐름 |
| [PuzzleRoot.cs](v2/Assets/_Projects/Scripts/Gameplay/Puzzle/PuzzleRoot.cs) | 퍼즐 기능 조립, 퍼즐·스킬 입력 전환, 발사·머지 결과의 후속 처리 |
| [ShooterRoot.cs](v2/Assets/_Projects/Scripts/Gameplay/Puzzle/SoulOrbShooter/ShooterRoot.cs) | 입력 해석기·발사 로직·ViewModel·View 조립 및 내부 표현 캡슐화 |

소유하는 컴포넌트는 인스펙터 참조로 구성하고, 실행 순서와 의존 관계가 중요한 연결은 **Initialize에서 직접 주입**했습니다. 발사·머지·게임 오버처럼 후속 처리 주체를 분리할 결과는 **R3 Observable**로 전달합니다.

```csharp
// PuzzleRoot.Initialize 일부 — 의존성 연결과 결과 구독을 한곳에서 구성
soulOrbQueueRoot.Initialize(monsterLoadout);
soulOrbRoot.Initialize(monsterLoadout, cameraController.RequestShake, requestHaptics);
shooterRoot.Initialize(soulOrbRoot.Spawner, playerInput);
gameOverRoot.Initialize();

skillRoot.Initialize(soulOrbRoot, comboRoot, shooterRoot, skillLoadout);

_addScore = addScore;

shooterRoot.Fired.Subscribe(OnShooterFired).AddTo(this);
soulOrbRoot.MergeFinished.Subscribe(OnSoulOrbMerged).AddTo(this);
soulOrbRoot.GameOverOrbPopped.Subscribe(OnGameOverSoulOrbPopped).AddTo(this);
```

예를 들어 머지가 끝나면 **PuzzleRoot가 점수·콤보·스킬 충전·이펙트의 후속 흐름을 조율**합니다. 머지를 수행하는 시스템은 이 후속 처리 전체를 직접 알 필요가 없습니다. 구독은 `AddTo(this)`로 소유 객체의 수명에 연결했습니다.

또한 GameplayRoot가 상태별로 `TickPlaying`·`TickGameOver`를 호출하고, PuzzleRoot가 하위 기능의 실행 순서를 정합니다. 이로써 조립뿐 아니라 **언제 어떤 기능을 갱신하는지도 코드에서 추적**할 수 있도록 구성했습니다.

### 3) 입력 구조 — 장치와 조작 방식의 변경 요인 분리

'댕댕이 서바이벌'에서는 모든 적을 거대한 `EnemyBase`로 상속시켰다가, 자식이 사용하지 않는 로직까지 포함하고 기반 클래스의 변경이 전체 적에게 영향을 미치는 문제를 겪었습니다.

v2의 입력 시스템에서는 이 경험을 바탕으로 **장치별 입력 변환과 조작 방식별 해석을 각각 분리**했습니다.

| 단계 | 구현 | 역할 |
|------|------|------|
| 장치 입력 | [GameplayPlayerInput.cs](v2/Assets/_Projects/Scripts/Gameplay/Input/GameplayPlayerInput.cs) · [PointerShooterInputReader.cs](v2/Assets/_Projects/Scripts/Gameplay/Input/Shooter/PointerShooterInputReader.cs) · [GamepadShooterInputReader.cs](v2/Assets/_Projects/Scripts/Gameplay/Input/Shooter/GamepadShooterInputReader.cs) | 장치에 맞는 Reader 선택, 조준 값과 버튼 상태를 공통 ShooterInputSnapshot으로 변환 |
| 조작 해석 | [ShooterInputResolver.cs](v2/Assets/_Projects/Scripts/Gameplay/Puzzle/SoulOrbShooter/Input/ShooterInputResolver.cs) · [IShooterInputStrategy.cs](v2/Assets/_Projects/Scripts/Gameplay/Puzzle/SoulOrbShooter/Input/Mode/IShooterInputStrategy.cs) | 설정에 따라 Direct·Drag 전략 선택, 조준·발사 의도로 해석 |
| 발사 실행 | [SoulOrbShooter.cs](v2/Assets/_Projects/Scripts/Gameplay/Puzzle/SoulOrbShooter/SoulOrbShooter.cs) | ResolvedShooterInput을 받아 발사 상태와 실제 발사 처리 |

```text
마우스·게임패드
  → 장치별 Reader
  → ShooterInputSnapshot
  → Direct·Drag 전략
  → ResolvedShooterInput
  → SoulOrbShooter
```

```csharp
// ShooterInputResolver.cs — 조작 방식에 맞는 전략으로 해석을 위임
private IShooterInputStrategy GetStrategy(ShooterControlMode controlMode) =>
    controlMode switch
    {
        ShooterControlMode.Direct => _directInputStrategy,
        ShooterControlMode.Drag => _dragInputStrategy,
        _ => throw new NotSupportedException($"{controlMode} is not supported."),
    };

public ResolvedShooterInput Resolve(in ShooterInputSnapshot input, float deltaTime)
{
    return _currentStrategy.Resolve(in input, deltaTime);
}
```

Direct 전략은 입력 방향과 버튼을 누른 시점으로, Drag 전략은 드래그 시작점과 이동량·버튼을 놓은 시점으로 조준과 발사 의도를 결정합니다. 조작 방식 변경 시에는 기존 전략의 `Exit`와 새 전략의 `Enter`를 호출해 상태 전환을 관리합니다.

발사 로직은 입력 장치나 전략의 구체 타입을 참조하지 않습니다. 따라서 **장치 입력의 변경은 Reader 영역에서, 조작 방식의 변경은 전략 영역에서 관리**할 수 있습니다.

**결론** — 결합도를 무조건 낮추거나 패턴을 많이 적용하는 것이 좋은 설계는 아니었습니다. 의존 관계가 명확한 경우 직접 연결하고, 실제로 변화하거나 분리할 이유가 있는 지점에 추상화를 적용하는 것을 기준으로 삼았습니다.

---

## 핵심 2 — 반복 작업 자동화: 에디터 유틸리티 툴

AI를 보조 수단으로 활용해 툴 요구사항을 명세하고 코드를 빠르게 반복 개선하면서, 툴 제작 비용을 크게 낮췄습니다. 덕분에 반복 잦던 그래픽 에셋 파이프라인을 유틸리티 툴로 적극 대체했습니다.

### 스프라이트 슬라이서 · [SpriteRowSlicerWindow.cs](v2/Assets/_TBM/Sprites/Editor/SpriteRowSlicerWindow.cs)

<img src="https://github.com/user-attachments/assets/f9d71119-3077-45aa-9bfd-a8331f1aea72" width="560" />

유니티 기본 에디터가 지원하지 않는 **'행 단위 시트 선택적 슬라이스'** 툴.
- **기존 문제** — 타겟 행 선택 추출 불가, 단순 숫자 네이밍 탓에 수작업 분류 비효율
- **주요 기능** — 프리셋을 통한 행별 일괄 네이밍 + 타겟 행 추출
- **성과** — 객체당 5분+ → **30초 이내**

### 스프라이트 그룹 피벗 에디터 · [SpritePivotGroupEditor.cs](v2/Assets/_TBM/Sprites/Editor/SpritePivotGroupEditor.cs)

<img src="https://github.com/user-attachments/assets/ab1e9632-75e8-4dbf-a277-24efd81d8f04" width="560" />

추출한 행 단위 스프라이트의 피벗을 **그룹 단위로 일괄 설정**하는 툴.
- **기존 문제** — 스냅·그룹 설정 부재, 피벗 대신 Root Transform으로 우회
- **주요 기능** — 도트 그래픽에 맞춘 0.5픽셀 단위 스냅으로 그룹 피벗 일괄 설정
- **성과** — 우회용 Root Transform 제거 → 오브젝트 계층 **1단계 축소**

### 애니메이션 타일 생성기 · [AnimatedTileCreatorWindow.cs](v2/Assets/_TBM/Tilemaps/Editor/AnimatedTileCreatorWindow.cs)

<img src="https://github.com/user-attachments/assets/745e6ac3-532f-497c-a787-5058cc8127d9" width="560" />

스프라이트 시트에서 애니메이션 프레임을 선택해 타일을 생성하는 툴.
- **기존 문제** — 수많은 스프라이트 중 필요 프레임을 일일이 찾아 드래그하는 비효율
- **주요 기능** — 시트 내 타일을 클릭·드래그로 그룹화해 단일·다중 애니메이션 타일 생성
- **성과** — 이름 찾는 과정 없이 시각화된 프레임 클릭·드래그만으로 타일 생성

**결론** — 단순 반복에 낭비되던 시간을 핵심 로직 개발에 재투자하게 됐고, 무엇보다 *'수작업 vs 툴 자동화'* 를 합리적으로 판단하는 기준이 자리 잡았습니다.

---

## 핵심 코드 구조

### v2 — 책임별 Root 구성 및 입력 전략 분리

<pre>
v2/Assets/
├── _Projects/Scripts/Gameplay/
│   ├── <a href="v2/Assets/_Projects/Scripts/Gameplay/GameplayRoot.cs">GameplayRoot.cs</a>                         # 전체 상태·생명주기 관리 ★
│   ├── Input/
│   │   ├── <a href="v2/Assets/_Projects/Scripts/Gameplay/Input/GameplayPlayerInput.cs">GameplayPlayerInput.cs</a>              # 액션 맵 관리·장치별 Reader 선택
│   │   └── <a href="v2/Assets/_Projects/Scripts/Gameplay/Input/Shooter">Shooter/</a>                           # Pointer·Gamepad Reader
│   └── Puzzle/
│       ├── <a href="v2/Assets/_Projects/Scripts/Gameplay/Puzzle/PuzzleRoot.cs">PuzzleRoot.cs</a>                       # 기능 조립·결과 후속 처리 ★
│       ├── SoulOrbShooter/
│       │   ├── <a href="v2/Assets/_Projects/Scripts/Gameplay/Puzzle/SoulOrbShooter/ShooterRoot.cs">ShooterRoot.cs</a>                  # 발사 로직·View 캡슐화
│       │   ├── <a href="v2/Assets/_Projects/Scripts/Gameplay/Puzzle/SoulOrbShooter/SoulOrbShooter.cs">SoulOrbShooter.cs</a>               # 해석된 입력으로 발사 처리
│       │   └── Input/
│       │       ├── <a href="v2/Assets/_Projects/Scripts/Gameplay/Puzzle/SoulOrbShooter/Input/ShooterInputResolver.cs">ShooterInputResolver.cs</a>     # 조작 방식별 전략 선택 ★
│       │       ├── <a href="v2/Assets/_Projects/Scripts/Gameplay/Puzzle/SoulOrbShooter/Input/ResolvedShooterInput.cs">ResolvedShooterInput.cs</a>     # 조준·발사 의도
│       │       └── <a href="v2/Assets/_Projects/Scripts/Gameplay/Puzzle/SoulOrbShooter/Input/Mode">Mode/</a>                      # 전략 계약·Direct·Drag
│       ├── <a href="v2/Assets/_Projects/Scripts/Gameplay/Puzzle/SoulOrb/SoulOrbRoot.cs">SoulOrb/SoulOrbRoot.cs</a>           # 생성·머지·제거 흐름
│       └── <a href="v2/Assets/_Projects/Scripts/Gameplay/Puzzle/Skill/SkillRoot.cs">Skill/SkillRoot.cs</a>               # 스킬 사용·충전·입력 흐름
└── _TBM/
    ├── Sprites/Editor/
    │   ├── <a href="v2/Assets/_TBM/Sprites/Editor/SpriteRowSlicerWindow.cs">SpriteRowSlicerWindow.cs</a>         # 행 단위 슬라이서
    │   └── <a href="v2/Assets/_TBM/Sprites/Editor/SpritePivotGroupEditor.cs">SpritePivotGroupEditor.cs</a>        # 그룹 피벗·0.5픽셀 스냅
    └── Tilemaps/Editor/
        └── <a href="v2/Assets/_TBM/Tilemaps/Editor/AnimatedTileCreatorWindow.cs">AnimatedTileCreatorWindow.cs</a>     # 애니메이션 타일 생성
</pre>

### v1 — 출시 버전 및 SO 이벤트 채널 비교

<pre>
v1/
├── <a href="v1/SO">SO/</a>                                    # ScriptableObject 이벤트·변수 채널
├── Stage/
│   └── <a href="v1/Stage/Score/ScoreManager.cs">Score/ScoreManager.cs</a>              # SO 기반 점수·콤보 연결 ★
└── <a href="v1/System">System/</a>                                # Audio·UI·Scene·플랫폼 서비스
</pre>
