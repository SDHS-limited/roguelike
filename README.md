# 🕹️ MAIP — 로그라이크 FPS
 
> **"강해질수록 통제력을 잃는다"**  
> Unity 6 기반 1인칭 슈팅 로그라이크 — 실험 선택으로 성장하지만, 그 대가로 폭주와 후유증이 쌓이는 리스크 기반 구조
 졸업작품
---
 
## 📌 프로젝트 개요
 
MAIP는 실험체로 납치된 천사 **라엘**을 조종해 탈출하는 로그라이크 FPS입니다.  
라운드마다 등장하는 **실험 카드**를 선택해 능력을 강화하지만, 선택할수록 **폭주 게이지**가 쌓이고 **후유증**이 발생해 조작이 점점 어려워집니다.  
강화와 붕괴가 동시에 누적되는 리스크 순환 구조가 핵심입니다.

개발기간 : 2026.03~
핵심역할 : 팀장, 메인 게임 기능 제작
---
 
## 🛠️ 기술 스택
 
| 항목 | 내용 |
|------|------|
| **엔진** | Unity 6 (6000.4.1f1) |
| **렌더 파이프라인** | URP (Universal Render Pipeline) |
| **언어** | C# |
| **AI** | Unity NavMesh Agent |
| **UI** | TextMeshPro, UI Animator, Slider |
| **맵 생성** | BSP(Binary Space Partitioning) 절차적 생성 |
| **기타** | ScriptableObject, 오브젝트 풀링 |
 
---
 
## ✨ 주요 구현 내용
 
### 1. 실험 시스템 (`ExperimentManager`, `Experiment`)
게임의 핵심 루프인 **실험 카드 선택 시스템**을 ScriptableObject 기반으로 구현했습니다.
 
- **ScriptableObject 기반 카드 데이터** — `Experiment` SO에 이름·설명·ID를 정의해 데이터와 로직 분리
- **중복 없는 랜덤 3장 제시** — 매 선택마다 풀에서 중복 없이 3개 추출, 선택 후 즉시 리셋
- **ID 기반 효과 분기** — `ApplyEffect` 코루틴에서 `experimentID`로 플레이어·총·이동·폭주 게이지를 직접 조작
- **카드 호버 애니메이션** (`SeleteCardAnim`) — `IPointerEnterHandler`로 마우스 오버 시 카드 상승 연출, `SmoothStep` 커브로 부드러운 이동
- **선택 카드 퇴장 애니메이션** — 선택된 카드가 위로 날아가며 알파값 0으로 페이드아웃, 코루틴으로 타이밍 제어
```csharp
// ScriptableObject로 실험 카드 데이터 정의
[CreateAssetMenu(menuName = "Scriptable Objects/Experiment")]
public class Experiment : ScriptableObject
{
    public string name;
    public string Des;
    public int experimentID;
}
```
 
### 2. 폭주(Berserk) & 후유증 시스템 (`AftereffectManager`, `Fever_Slider`)
기획서의 핵심 콘셉트인 **"강해질수록 통제력을 잃는다"** 를 코드로 구현했습니다.
 
- **폭주 게이지** (`Fever_Slider`) — `currentFever`가 `maxFever(100)` 도달 시 `OnBerserkReached` C# 이벤트 발동
- **게이지 비율(feverRatio) 실시간 전달** — `AftereffectManager`가 매 프레임 비율을 `Recoil`에 전달해 반동 크기 동적 증가
- **폭주 모드** — 10초간 공격력 2배·무한 탄약 자동연사 + 매초 체력 감소, 종료 후 게이지 50%로 리셋
- **후유증 랜덤 발동** — 게이지 20% 이상 시 발동 간격 단축, 3가지 유형 랜덤 선택
  - **이동 후유증** — 0.5~1.5초 이동속도 30% 감소 + 방향 없는 비틀거림 힘 적용
  - **전투 후유증** — 강제 발사 또는 총기 잼(발사 불가) 상태 부여
  - **시점 후유증** — 카메라 방향 강제 트위치, 반동 배율로 연동
### 3. 반동 시스템 (`Recoil`)
**폭주 게이지와 연동되는 동적 반동**을 구현했습니다.
 
- **Vector3 Lerp 복귀** — `targetRotation`을 `baseRotation`으로 Lerp, `currentRotation`을 Slerp로 부드럽게 추적
- **feverRatio 연동** — 폭주 게이지 비율만큼 반동 강도 비례 증가 (`recoilX * (1 + feverRatio)`)
- **발사 조건 검사** (`CanFire`) — 현재 회전이 기준값 오차 `threshold` 이내일 때만 연사 허용, 반동 중 발사 방지
- **ApplyTwitch** — 후유증 시스템에서 호출, X·Y축 랜덤 오프셋으로 시점 강제 흔들림
### 4. 방향별 더블탭 대시 (`Player`)
**WASD 각 방향 독립 쿨다운**을 가진 더블탭 대시를 구현했습니다.
 
- 각 키의 마지막 탭 시각(`lastTapW/A/S/D`)을 기록해 `dashDoubleTapWindow(0.25s)` 이내 재입력 시 발동
- 4방향 각각 독립 쿨다운 타이머, 벽 충돌은 `Raycast`로 선행 감지해 관통 방지
- `SmoothStep` 커브 기반 대시 코루틴으로 시작·끝이 부드러운 이동 구현
### 5. BSP 절차적 맵 생성 (`CreateMap`)
**Binary Space Partitioning** 알고리즘으로 매 플레이마다 다른 던전을 생성합니다.
 
- 전체 공간을 `maxDepth(4)`까지 재귀 분할, 가로·세로 비율에 따라 수직·수평 분할 방향 자동 결정
- 분할된 리프 노드에 랜덤 크기 방 생성, 이웃 방 중심점 연결하는 L자 복도 생성
- `int[,] map` 2D 배열로 바닥(1) / 벽(2) / 복도(3)을 마킹 후 일괄 Instantiate
```csharp
void Divide(BSPNode node, int depth)
{
    bool splitVertical = node.rect.width >= node.rect.height;
    float ratio = Random.Range(0.4f, 0.7f);
    // 재귀 분할 → 리프 노드에서 방 생성
}
```
 
### 6. 미니맵 & 전체맵 시스템 (`MinimapManeger`, `FullMapManager`)
**월드 좌표 → UI 좌표** 변환 기반의 실시간 미니맵을 구현했습니다.
 
- 미니맵 전용 직교 카메라의 `WorldToViewportPoint`로 Viewport 좌표 계산 후 `iconPanel.rect` 크기에 맞게 스케일
- 적 생성 시 `RegisterMonster`, 사망 시 `UnregisterMonster` 호출로 아이콘 동적 추가·제거
- `Dictionary<Enemy, RectTransform>`으로 적 인스턴스↔아이콘 1:1 매핑, 매 프레임 위치 동기화
- Tab 키로 전체맵 Canvas 토글
### 7. 적 AI (`Enemy`, `BuildingEnemy`, `suicide`)
기획서의 적 역할 설계를 NavMesh 기반으로 구현했습니다.
 
| 적 타입 | 구현 내용 |
|---------|----------|
| **근접형 (Enemy)** | NavMeshAgent 추적, 애니메이터 연동(Move/Attack), 공격 범위 내 접근 시 정지 |
| **건물형 (BuildingEnemy)** | 고정 위치에서 감지 범위 내 플레이어에게 발사체 조준 사격 |
| **자폭형 (suicide)** | 접촉 시 자신 파괴 + 플레이어 데미지, NavMesh 추적 |
 
- 근접형 피격 시 `DamageTextManager` 풀에서 텍스트 오브젝트 꺼내 피해량 팝업 표시
### 8. 총기 시스템 (`Gun`)
장전 모션과 폭주 연동이 포함된 총기 시스템입니다.
 
- **장전 애니메이션** — 총 오브젝트 X축 360도 회전 코루틴 + `Reload_Slider` UI 동기화
- **잼(Jamming) 상태** — 후유증 시스템에서 주입, 발사·장전 모두 차단
- **폭주 연사 모드** — 탄약 고정 7발 유지, 0.1초 간격 자동 발사
### 9. 이동 시스템 (`Move`)
CharacterController 기반의 FPS 이동 시스템입니다.
 
- 달리기·걷기·앉기 속도 분리, `Acceleration/Deceleration` 기반 Lerp로 관성 표현
- 앉기 시 `CharacterController.height` + `Center` 동시 보정, 머리 위 SphereCast로 기립 불가 체크
- 이동 속도에 비례한 헤드밥(HeadBob) — `Mathf.Sin`으로 Y축 카메라 흔들림
### 10. 방 클리어 & 문 시스템
- **방 클리어** (`room`) — 자식 Enemy 수 추적, 전부 사망 시 타이핑 이펙트로 클리어 텍스트 출력
- **자동 문 개폐** (`Door`) — 플레이어가 어느 방향에서 접근했는지 X좌표 비교로 판별, Slerp로 방향에 맞게 회전
- **트랩** (`SlowedTrap`) — OverlapSphere로 플레이어 감지, 범위 진입 시 이동속도 50% 감소·이탈 시 복구
---
 
## 🏗️ 프로젝트 구조
 
```
Assets/Script/
├── Manager/
│   ├── Player.cs              # 플레이어 스탯, 방향별 더블탭 대시
│   ├── Move.cs                # CharacterController 이동, 앉기, 헤드밥
│   ├── AftereffectManager.cs  # 후유증 랜덤 발동, 폭주 루틴
│   ├── RestManager.cs         # 휴식방 체력 회복
│   └── ShopMananger.cs        # 상점 (개발 예정)
│
├── Experiment/
│   ├── Experiment.cs          # ScriptableObject 카드 데이터
│   ├── ExperimentManager.cs   # 랜덤 카드 제시, 효과 적용
│   ├── ExperimentObj.cs       # 실험 장치 오브젝트 (접근 감지, UI 토글)
│   └── SeleteCardAnim.cs      # 카드 호버·선택 UI 애니메이션
│
├── Enemy/
│   ├── Enemy.cs               # 근접 추적 AI, 피격, 미니맵 등록
│   ├── BuildingEnemy.cs       # 고정 포대형 원거리 적
│   └── suicide.cs             # 자폭형 적
│
├── Weapon/
│   ├── Gun.cs                 # 발사, 장전, 잼, 폭주 연사
│   └── Sw.cs                  # 보조 무기 (프로토타입)
│
├── Bullet/
│   ├── Bullet.cs              # 플레이어 총알 (Rigidbody, 충돌 파티클)
│   └── BuildBullet.cs         # 건물형 적 발사체
│
├── Camera/
│   ├── CameraRot.cs           # 마우스 입력 FPS 카메라 (SmoothDamp)
│   ├── CameraShake.cs         # 발사 시 카메라 흔들림
│   └── HeadBobController.cs   # Sin 기반 헤드밥
│
├── Minimap/Script/MapCS/
│   ├── MinimapManeger.cs      # 미니맵 아이콘 관리 (플레이어·적)
│   ├── FullMapManager.cs      # 전체맵 아이콘 관리
│   ├── MinimapCamera.cs       # 플레이어 추적 미니맵 카메라
│   └── FullMapToggle.cs       # Tab 키 전체맵 토글
│
├── UI System/
│   ├── Fever_Slider.cs        # 폭주 게이지 (이벤트 기반 상태 관리)
│   ├── HP_Slider.cs           # 체력 슬라이더
│   ├── DamageTextManager.cs   # 피해 텍스트 오브젝트 풀
│   ├── DamageTextEffect.cs    # 피해 텍스트 연출 (Float + FadeOut)
│   └── Effect.cs              # 피격·회복 화면 플래시 이펙트
│
├── Trap/
│   ├── SlowedTrap.cs          # 속도 감소 트랩
│   └── thornTrap.cs           # 가시 트랩 (개발 예정)
│
├── Recoil.cs                  # 반동 애니메이션 + 폭주 연동
├── CreateMap.cs               # BSP 절차적 던전 생성
├── Door.cs                    # 접근 방향 감지 자동 문 개폐
└── room.cs                    # 방 클리어 감지 및 텍스트 연출
```
 
---
 
## 🎮 기획 핵심 시스템
 
| 시스템 | 설계 원칙 |
|--------|----------|
| **실험 카드** | 항상 트레이드오프 존재 — 강화와 폭주 게이지 상승이 동반 |
| **폭주** | 보상이 아닌 위험 — 10초간 강해지지만 체력 감소 + 조작 불안정 |
| **후유증** | 게이지 비율에 따라 발동 빈도·강도 비례 증가 |
| **엔딩 분기** | 별도 선택지 없이 플레이 패턴(폭주 의존도)에 따라 자연 결정 |
 
---
 
## 💡 기술적 도전과 해결
 
**기획과 코드 연결 — 서사와 시스템의 일체화**  
→ 라엘이 인간성을 잃는 서사를 `feverRatio`가 높아질수록 반동·후유증이 심해지는 코드로 직접 표현
 
**ScriptableObject로 카드 데이터와 로직 분리**  
→ 디자이너가 Unity Inspector에서 카드 이름·설명·ID만 설정하면 `switch(experimentID)`로 자동 효과 적용
 
**BSP 맵 생성 시 복도 연결 누락 문제**  
→ 리프 노드뿐만 아니라 부모 노드 단위에서 재귀적으로 자식 방 중심점 연결, 모든 방이 연결되도록 보장
 
**피해 텍스트 GC 최소화**  
→ `DamageTextManager` 싱글톤에서 `Queue<GameObject>` 풀 관리, `SetActive`로 재사용
 
---
 
