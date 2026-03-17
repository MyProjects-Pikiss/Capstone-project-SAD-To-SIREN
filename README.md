# SAD TO SIREN

## Overview
Unity 기반 2D 로그라이크 생존 게임입니다.  
탐사, 전투, 자원 관리, 성장 요소를 결합한 반복 플레이 구조를 중심으로 설계되었습니다.

## Gameplay Loop
1. 필드 탐사
2. 자원 수집 및 전투
3. 인벤토리 관리 및 장비 정비
4. 캐릭터 및 유닛 강화
5. 다음 탐사 진행

## Core Systems

### Inventory & Crafting
- 아이템 제작 및 인벤토리 관리
- 장비 장착 및 능력치 반영

### Player State System
- 체력, 스태미너 등 상태 관리
- 시간 흐름에 따른 변화 처리

### Building System
- 건물 설치 및 업그레이드
- 게임 진행도 기반 확장

### Unit & Equipment
- 유닛 강화 시스템
- 장비 기반 전투 성능 변화

### Procedural Generation
- 랜덤 맵 생성
- 랜덤 루팅 시스템

## Pathfinding
- A* 알고리즘 기반 유닛 이동 구현
- 육각형 타일 구조에 맞게 경로 탐색 로직 적용

## Tech Stack
- Unity
- C#
- Stable Diffusion (게임 리소스 생성)
