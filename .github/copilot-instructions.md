프로젝트 핵심 요약
- 엔진: Unity (버전 확인: `ProjectSettings/ProjectVersion.txt` — 현재 `m_EditorVersion: 6000.3.7f1`).
- 구조: 모든 게임 로직은 `Assets/Script/` 아래의 MonoBehaviour 스크립트로 구성됨. 주요 폴더: `Manager`, `Enemy`, `Experiment`, `UI System`, `Minimap`, `Trap`, `weapn` 등.

바로 시작하기 (에이전트가 빠르게 작업하려면)
- 에디터에서 프로젝트를 연 뒤(위의 ProjectVersion 일치 권장). 스크립트 컴파일/런타임 동작은 Unity Editor 콘솔에서 확인.
- 로컬 코드 검사: 솔루션/프로젝트 파일(`.sln`, `Assembly-CSharp.csproj`)이 존재하므로 IDE(Visual Studio / Rider / VSCode)는 컴파일 타임 검사에 유용.

아키텍처와 데이터 흐름(핵심 포인트)
- 게임 루프와 입력: `Assets/Script/Manager/Player.cs` — 입력 처리(`Update`), 이동(`transform.Translate`), 대시(레이캐스트로 장애물 검사 후 코루틴으로 처리).
- 능력/카드 시스템: `Assets/Script/Experiment/ExperimentManager.cs` — 인스펙터에서 `allExperiments` 배열로 능력(데이터)을 할당하고, 선택 시 `ApplyEffect` 코루틴으로 게임 오브젝트(예: `Player`, `Bullet`, `Fever_Slider`) 속성을 변경.
- 맵 생성: `Assets/Script/CreateMap.cs` — BSP 기반 방 생성 + 코릴도어 연결, 프리팹 Instantiate 방식으로 타일 빌드.
- 적/아이콘 등록: `Assets/Script/Enemy/Enemy.cs` — 생성 시 미니맵/풀맵 매니저에 등록(`RegisterMonster`), 파괴 시 해제. 중요한 패턴: `FindFirstObjectByType(..., FindObjectsInactive.Include)`로 비활성화된 매니저도 찾음.

프로젝트 규약과 반복 패턴(에이전트가 따라야 할 규칙)
- 인스펙터 의존성: 대부분 컴포넌트가 `[SerializeField]`로 연결되어 있음(예: `Player.damage`, `ExperimentManager.allExperiments`, `CreateMap.floorPrefab`). 스크립트 변경 시 이름이나 타입을 바꾸면 인스펙터 연결이 깨질 수 있으니 주의.
- 태그/충돌: 충돌 판정은 `CompareTag("Enemy")`, `CompareTag("Player")`, `CompareTag("Bullet")` 같은 태그 기반 로직에 의존. 태그 이름을 변경하지 마세요.
- 코루틴/애니메이션: 상태 변화는 종종 `StartCoroutine(...)`와 애니메이션 대기(`yield return new WaitForSeconds(...)`)로 구현됨. 동기/비동기 흐름을 바꿀 때는 호출 지점을 확인.
- 내비(Pathfinding): `NavMeshAgent`를 사용. 적은 `nav.SetDestination(target.position)` 방식으로 추적.

실전 예시(참고 파일/라인)
- 플레이어 대시: `Assets/Script/Manager/Player.cs` — Raycast로 충돌 확인 후 `DashCoroutine`으로 부드럽게 보간.
- 능력 적용: `Assets/Script/Experiment/ExperimentManager.cs` — `ApplyEffect` 스위치 문으로 게임 상태 변경(예: `bullet.Damage += 10`, `player.speed -= 2f`).
- 몬스터 등록: `Assets/Script/Enemy/Enemy.cs` — `RegisterToMaps()` / `UnregisterFromMaps()`.

빌드 · 실행 관련(발견 가능한 워크플로우)
- 개발 루프: Unity Editor에서 플레이(Play)로 확인. 에디터에서 스크립트 변경 → 자동 컴파일 → 플레이 재시작 확인.
- IDE: `.sln` / `Assembly-CSharp.csproj`로 코드 편집/정적 검사. 패키지 정보는 `Packages/manifest.json`에 있음(예: `com.unity.inputsystem`, `com.unity.ai.navigation`, `com.unity.visualscripting`).

주의 사항(탐지된 프로젝트 특이점)
- 폴더/파일명 불일치: 일부 네이밍 오타(`ShopMananger.cs`)가 있음 — 변경 시 인스펙터 연결과 참조를 확인하세요.
- 폴더 이름에 공백(`UI System`)이 있음 — 상대 경로나 에셋 경로 작업 시 주의.
- 비활성 오브젝트 검색: 코드가 `FindFirstObjectByType(..., FindObjectsInactive.Include)`를 사용하므로, 일부 매니저는 비활성 상태여도 검색되어 등록/해제 로직이 동작함.

작업 가이드(예: 기능 추가/버그 수정할 때 단계)
1. 관련 매니저/스크립트를 찾기: `Assets/Script/Manager/`와 `Assets/Script/Experiment/` 먼저 확인.
2. 인스펙터 연결 확인: `[SerializeField]` 필드가 에디터에서 올바르게 연결되어 있는지 확인.
3. 런타임 동작 확인: Unity Editor에서 Play를 눌러 콘솔 로그/스택트레이스 확인.
4. 작은 변경 후 즉시 테스트: 코루틴/물리/태그는 런타임에만 완전히 검증 가능.

피드백 요청
이 파일을 업데이트했어요. 더 추가할 파일(예: 특정 ScriptableObject 정의나 씬 구성)이나, 에이전트가 자동으로 수행하길 원하는 작업(예: 자동 컴파일 후 유닛 테스트 실행)이 있으면 알려주세요.
