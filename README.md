# 가위바위보 (Unity)

Unity 기반의 **가위·바위·보** 대전 게임입니다. 플레이어는 UI 버튼으로 선택하고, 컴퓨터는 무작위로 출전합니다.

## 게임 규칙

- **승리**: 플레이어 또는 컴퓨터 중 가위바위보를 이긴 사람이 승리합니다.
- **패배**: 플레이어 또는 컴퓨터 중 가위바위보를 진 사람이 패배합니다.
- **무승부**는 점수에 반영되지 않습니다. 승리 시에만 해당 쪽 점수가 오릅니다.
- 대기 중에는 컴퓨터 쪽 이미지에 가위/바위/보 스프라이트가 **순환**되어 표시됩니다. 플레이어가 선택하면 애니메이션이 멈추고 승부가 납니다.

## 조작·버튼

| 버튼 | 동작 |
|------|------|
| 가위 / 바위 / 보 | 한 판 진행 (컴퓨터는 랜덤 선택) |
| 재시작 | **다시** 재시작 |
| 종료 | 에디터에서는 플레이 모드 종료, 빌드에서는 `Application.Quit()` |

승패 결과와 안내 문구는 **TextMeshPro** UI로 표시되며, `GameManager`에서 버튼·이미지·점수·결과 패널을 참조합니다.

## 핵심 스크립트

- `Assets/Scripts/GameManager.cs` — 선택 처리, 랜덤 AI, 승패 판정, 점수·종료 조건(5점), UI 연동

## `Assets/` 폴더 구조

```
Assets/
├── Animation/          # 버튼용 Animator Controller (가위/바위/보)
├── Fonts/              # 커스텀 폰트 및 TMP SDF 에셋
├── Scenes/             # SampleScene.unity, Scene2.unity
├── Scripts/            # GameManager.cs, Exam.cs
├── Settings/           # URP 2D 렌더러·파이프라인·씬 템플릿
│   └── Scenes/
├── Sprites/            # rock, paper, scissors, question_mark 등 이미지
├── TextMesh Pro/       # TMP 패키지 리소스 (폰트, 셰이더, 리소스 등)
│   ├── Fonts/
│   ├── Resources/
│   │   ├── Fonts & Materials/
│   │   ├── Sprite Assets/
│   │   └── Style Sheets/
│   ├── Shaders/
│   └── Sprites/
├── _Recovery/          # Unity 복구용 씬 (0.unity)
├── DefaultVolumeProfile.asset
├── InputSystem_Actions.inputactions
├── UniversalRenderPipelineGlobalSettings.asset
└── plan.md             # 기획/노트용 마크다운
```

## 실행 방법

1. Unity 에디터에서 이 프로젝트 폴더를 엽니다.
2. `Assets/Scenes/` 중 원하는 씬을 연 뒤 **Play**로 실행합니다.