# 마왕 수박 게임
> 써드파티 에셋 라이센스로 인해 전체 프로젝트가 아닌 C# 스크립트만 포함

> 수박 게임 류 | Unity | 1인 개발 | 안드로이드 | 출시

- v1 : 출시 후 서비스 종료
- v2 : 개선 및 차기작(미출시)

### 트레일러
[![트레일러](https://i9.ytimg.com/vi_webp/L1Ubq69U_OY/mqdefault.webp?v=6639077b&sqp=CIyWxs4G&rs=AOn4CLChaSKVkkZhtPTtLqW0hl9AjcZu2g)](https://www.youtube.com/watch?v=L1Ubq69U_OY)


## v1 핵심 구현

### SO 이벤트 채널
의존성 필요한 모든 클래스에 이벤트 채널을 사용
[ScoreManager.cs](v1/Stage/Score/ScoreManager.cs), [GameOverSystem.cs](v1/Stage/GameOver/GameOverSystem.cs) 등

---

## v2 핵심 구현
### 전략 패턴 입력 시스템

```mermaid
flowchart TD
    Device["🎮 디바이스\n(터치 / 마우스 / 키보드)"]
    UIS["Unity Input System\n(.inputactions)"]
    IC["InputController"]
    PC["PuzzleController (중재자)"]
    PIS["PuzzleInputSystem\nShooterInputHandler / SkillInputHandler"]
    SS["ShooterSystem"]
    SIM["ShooterInputModule"]
    STRAT["«interface» IShooterInputStrategy\nDrag / Classic / Direct / VirtualCursor / None"]

    Device --> UIS --> IC --> PC
    PC --> PIS
    PIS --> SS
    SS --> SIM --> STRAT
```

| 파일 | 역할 |
|------|------|
| [InputController.cs](v2/Core/Input/InputController.cs) | Unity Input System 래퍼, scheme/device 변경 이벤트 발행 |
| [PuzzleController.cs](v2/Gameplay/Puzzle/PuzzleController.cs) | 퍼즐 전체 흐름 조율, 입력값 polling 명령 및 전달 |
| [PuzzleInputSystem.cs](v2/Gameplay/Puzzle/Input/PuzzleInputSystem.cs) | 입력 시스템 진입점, ShooterInputHandler / SkillInputHandler 관리 |
| [ShooterInputHandler.cs](v2/Gameplay/Puzzle/Input/ShooterInputHandler.cs) | InputAction 폴링, DeadZone 처리(물리적) |
| [ShooterSystem.cs](v2/Gameplay/Puzzle/Shooter/ShooterSystem.cs) | 슈터 전체 로직 관리 |
| [ShooterInputModule.cs](v2/Gameplay/Puzzle/Shooter/Input/ShooterInputModule.cs) | 전략 패턴을 활용하여 입력 방식에 따라 전략 결정 |
| [IShooterInputStrategy.cs](v2/Gameplay/Puzzle/Shooter/Input/Strategy/IShooterInputStrategy.cs) | 입력 전략 인터페이스 |
| [ShooterDragStrategy.cs](v2/Gameplay/Puzzle/Shooter/Input/Strategy/ShooterDragStrategy.cs) | 드래그 입력 전략 |
| [ShooterClassicStrategy.cs](v2/Gameplay/Puzzle/Shooter/Input/Strategy/ShooterClassicStrategy.cs) | 클래식 입력 전략 |
| [ShooterDirectStrategy.cs](v2/Gameplay/Puzzle/Shooter/Input/Strategy/ShooterDirectStrategy.cs) | 다이렉트 입력 전략 |
| [ShooterVirtualCursorStrategy.cs](v2/Gameplay/Puzzle/Shooter/Input/Strategy/ShooterVirtualCursorStrategy.cs) | 가상 커서 입력 전략 |
| [ShooterNoneStrategy.cs](v2/Gameplay/Puzzle/Shooter/Input/Strategy/ShooterNoneStrategy.cs) | None 전략 (입력 비활성화) |

### 아키텍처 개선

코드의 흐름이 명확해지고 필요한 책임만 져서 간결해짐

| v1 | v2 |
|----|----|
| [ScoreManager.cs](v1/Stage/Score/ScoreManager.cs) | [ScoreSystem.cs](v2/Gameplay/Puzzle/Score/ScoreSystem.cs) |
| | [ScoreModel.cs](v2/Gameplay/Puzzle/Score/ScoreModel.cs) |
| | [ScoreVisualizer.cs](v2/Gameplay/Puzzle/Score/ScoreVisualizer.cs) |


### 빌더 패턴

[PuzzleContext.cs](v2/Gameplay/Puzzle/PuzzleContext.cs) - 파라미터 길어짐 방지, 필수 값 강제

---

## 핵심 코드 구조

### v1

```
v1/
├── SO/                        # ScriptableObject 기반 이벤트/변수 채널
│   ├── 01_Variable/           # 변수 SO (Int, Float, Bool 등)
│   ├── 02_Data/               # 데이터 SO
│   └── 03_Event/              # 이벤트 채널 SO
├── Stage/                     # 게임 플레이
│   ├── Gashapon/              # 수박 오브젝트 (발사, 합체, 큐)
│   ├── Combo/                 # 콤보 시스템
│   ├── Score/                 # 점수
│   ├── Battle/                # 배틀
│   ├── Overlord/              # 마왕 반응형 대사
│   └── GameOver/              # 게임오버
└── System/                    # 공통 시스템 (Audio, UI, Scene 등)
```

### v2

```
v2/
└── Gameplay/                  # 게임 특화 로직
    └── Puzzle/
        ├── Input/             # 입력 시스템 ★
        ├── Shooter/           # 슈터 (전략 패턴) ★
        └── Score/             # 점수 (Model-View 분리)
```


