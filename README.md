## 미리보기  

***
[플레이 영상 보기(제작중)]

***

<img width="721" height="405" alt="image" src="https://github.com/user-attachments/assets/546bf1ec-03a0-4091-8606-122832889ef8" />

<img width="732" height="414" alt="image" src="https://github.com/user-attachments/assets/d31e9f2f-c32a-408c-921d-cc0fef684ca0" />

<img width="724" height="400" alt="image" src="https://github.com/user-attachments/assets/6e26d1ba-178a-4291-914d-084c21aec98d" />

<img width="702" height="383" alt="image" src="https://github.com/user-attachments/assets/cad95553-8ce8-4776-b8e0-ad2ab93d59a1" />


## 목차

[프로젝트 개요](#프로젝트-개요)  
[프로젝트 목표](#프로젝트-목표)  


[1. 랜덤 맵 생성 알고리즘](#1-랜덤-맵-생성-알고리즘)  
[2. 몬스터 움직임](#2-몬스터-움직임)  
[3. 메인 메뉴 연출](#3-메인-메뉴-연출)  
[4. 사운드](#4-사운드)  

[포트폴리오 관련 링크](#포트폴리오-관련-링크)  

***

## 프로젝트 개요
|항목|내용|
|------|---|
|프로젝트명|Unity_Isaac_Mojak|
|개발 기간|2025.8 - 2025.9 (1개월)|
|사용 Tool|Unity 2022.3.62f1, Visual Studio, NavMeshPlus, Input System, Cinemachine|
|목표|'아이작'을 모작하여 2D 동작구조와 랜덤 생성 알고리즘 구현|

***
## 프로젝트 목표
1. 랜덤 방 생성 알고리즘 구현  
2. 캐릭터, 아이템 등 상호작용 시스템 설계
3. UI 및 2D 애니메이션 제작

***

## 1. 랜덤 맵 생성 알고리즘
<details open>
  <summary>랜덤 맵 생성 알고리즘 내용(접기 / 펼치기)</summary>

[맵 생성 알고리즘 코드 주소](https://github.com/wjh6052/Unity_Isaac_Mojak/blob/3b9e5ad4680874d2a0be4a3a6b3fa652c016393a/Isaac_Mojak/Assets/02.Script/01.Game/Mgr/Mgr_Game.cs)

* 규칙 
1. 시작방은 (0,0)
2. 좌우 이동시 x + 4.2, 상하 이동 시 y + 2.4
3. 전체 방 개수는 minRooms ~ maxRooms 범위에서 랜덤
4. 노멀 방은 랜덤 워크 방식으로 확장
5. 보스방/황금방은 반드시 1개씩 보장되며 막다른 길에 위치해야함 (연결된 방이 1개뿐)
7. 보스방은 시작방과 이어지면 안됨

* 기본 구조
- 좌표계 : 2D 정수 그리드 시스템 (Vecotr2Int)
- 방 배치 : 좌우 4.2, 상하 2.4 간격
- 방 타입 : 시작방, 일반방, 보스방, 황금방 (열거형으로 타입을 관리하여 추가 가능)

* 생성 방식
- 초기화: (0,0)에서 시작방 고정 생성
- 확장 알고리즘: 랜덤 워크(Random Walk) 방식으로 맵 확장
- 방 개수: 최소 생성 개수와 최대 생성 개수를 변수로 저장하여 수정가능 (시작방 제외)
- 시드 시스템: 재현 가능한 랜덤 생성
- 특수 방 : 각 방의 조건에 따라 생성
- 랜덤 방 : 맵마다 각기다른 오브젝트를 배치하여 프리팹으로 생성 후 배열에 저장하여 생성시에 랜덤 Int값을 입력하여 생성하는 방식

* 기술적 특징
- 자료구조 : Dictionary<Vector2Int, Room_Ctrl>을 이용한 효율적 공간 관리
- 확장성: Enum 기반 방 타입 시스템으로 새로운 방 타입 추가 용이
- 재현성: 시드 기반 랜덤 생성으로 동일한 맵 재생산 가능
- 최적화: 방 활성화/비활성화 시스템으로 대규모 맵 처리
- 연결성: 완전 연결 그래프 구조 보장


<summary>미니맵 구현 (접기 / 펼치기)</summary>
<img width="112" height="148" alt="image" src="https://github.com/user-attachments/assets/c47071af-7973-49ec-8585-60e0533a8015" />

<img width="118" height="132" alt="image" src="https://github.com/user-attachments/assets/662a2cf3-3f4e-4757-a9c3-e64d1691d408" />

* 미니맵 작동원리
Dictionary<Vector2Int, Room_Ctrl>타입으로 저장된 스테이지 방 정보를 가져와 플레이어와 방의 상호작용 타입을 나눠 UI 아이콘으로 표시
    1. Current,    // 플레이 중 (현재 방)
    2. Visited,    // 들어갔던 적이 있음
    3. Discovered, // 가본 적 없음 (보이긴 하지만 아직 입장x)
    4. Hidden      // 안보임 (미발견)

* 미니맵의 업데이트 주기
- 스테이지 생성 시 최초 업로드
- 방 이동 시 


</details>

</details>


***

## 2. 몬스터 움직임
<details open>
  <summary>몬스터 움직임(접기 / 펼치기)</summary>

<img width="425" height="320" alt="아이작 네비 매쉬" src="https://github.com/user-attachments/assets/c5f5b499-5c5a-4b6c-afb4-c759f5021adf" />

* 2D Nav Mesh
- NavMeshPlus를 이용하여 X축을 -90으로 고정, 바닥 레이어를 설정하여 이동가능한 바닥을 판정
- 맵의 오브젝트의 경우 NavMeshObstacle를 이용하여 장애물을 판정하여 길을 돌아가도록 설정
- 맵 오브젝트의 경우 타일맵으로 설정하여 플레이시 타일맵의 정해둔 오브젝트 프리팹이 스폰되는 방식  
[방 오브젝트 타일맵 코드](https://github.com/wjh6052/Unity_Isaac_Mojak/blob/3b9e5ad4680874d2a0be4a3a6b3fa652c016393a/Isaac_Mojak/Assets/Tilemap/PrefabTile.cs)

</details>


***

## 3. 메인 메뉴 연출
<details open>
  <summary>메인 메뉴 연출 (접기 / 펼치기)</summary>
<img width="267" height="307" alt="메인메뉴 씬" src="https://github.com/user-attachments/assets/a6de356a-7b52-4cf7-8fab-65417cb775b3" />

* 카메라 연출
- 씬에 Sprite Renderer를 스폰하여 각각의 메뉴마다 이동하는 방식을 선택
- 시네머신 카메라를 이용하여 부드러운 카메라 이동을 연출
- 인게임과 동일하게 InputSystem를 이용하여 플레이어의 입력을 받아 메뉴를 조작할 수 있습니다


</details>

***

## 4. 사운드
<details open>
  <summary>사운드 시스템 (접기 / 펼치기)</summary>

[Mgr_Sound 코드](https://github.com/wjh6052/Unity_Isaac_Mojak/blob/3b9e5ad4680874d2a0be4a3a6b3fa652c016393a/Isaac_Mojak/Assets/02.Script/Mgr_Sound.cs)


+ 사운드를 관리할 Mgr_Sound를 싱글톤 패턴과 DontDestroyOnLoad를 통해 모든 씬에서 항상 존재하도록 설정했습니다

+ Mgr_Sound.Co_PlaySound인 코루틴 함수를 통해 (GameObject obj, AudioClip inAudioClip, bool isLoop = false)를 매개변수로 받아 obj에 AudioSource를 붙인 후 inAudioClip를 재생, 
volume은 전체 셋팅인 GlobalValue.SoundVolume에서 볼륨을 조절하여 실행합니다. isLoop가 true인 경우 오브젝트가 없어질때까지 실행하며 obj가 null이 될때까지 체크 후 코루틴함수를 종료합니다.

</details>

***

## 포트폴리오 관련 링크

[포트폴리오 영상 보기(제작중)]  
[게임 데모 다운로드 (구글드라이브)](https://drive.google.com/drive/folders/1w1hEu9wXIA5q2DQCFaIyyY16X1M6iEuQ)  
