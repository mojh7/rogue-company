﻿  
  ItemAsset 설명 및 실제 코드가 담긴 cs 파일에서 주석 지저분한 것들 지우고 이곳으로 내용 옮기기 위해서 따로 readme text 파일 만듬.



전체적인 클래스 흐름

UsableItem -> EffectApplyType -> ItemUseEffect

UsableItem이란?

item의 자식 클래스로 말 그대로 사용가능한 아이템으로 아이템을 썼을 때 어떠한 효과가 적용되는 아이템들이다.
어떤 형태로든 캐릭터 스탯이나 무기에 효과를 주는 아이템들
현재 일단 usableItem은 Player만 쓸 수 있도록 생각해서 만들 예정.

UsableItem 종류
 - 음식 FoodItem
 - 의류 ClothingItem
 - 잡화 MiscItem(패시브 전용)
 - 의료 MedicalItem
 - 펫 PetItem

 - 패시브 아이템 PassiveItem

UsabelItem에는 등급이 나뉘어있다.
 - 좋은 아이템 S ← A ← B ← C ← D ← E 덜 좋은 아이템 이다.

[공통점]
 - 습득 즉시 바로 사용되어 효과를 얻을 수 있다.
  
[차이점] - EffectApplyType으로 구분
 * 효과가 적용될 때 적용되는 방법에 차이가 있다.
 
-----------------------------------------------------------------------------------------

EffectApplyType이란?

usableItem을 얻게 되었을 때 즉시 사용되어서 아이템 효과가 적용되는데 (아이템 창, 아이템 퀵슬룻 같은 ui가 따로 있지 않음)
사용될 때 사용되는 형태, 효과가 적용되는 형태에 따라서 구분 지었다.
현재 3가지로 구분 되어 있고 특이한 경우가 생길 경우 따로 자식 클래스를 더 만들어서 구분 지으면 된다.


1. ConsumableType - 1회성 소모품 효과
 - 아이템 효과가 적용된 후 아이템 즉시 소멸
 - 1회성 소모품 자체가 저장 되기 보다는 소모품으로 인해 적용되고 나서의 스탯들이 적용됨.
 ex ) hp = 4 였다가 hp + 1 아이템 먹고 게임 데이터 저장이 이루어진다면 hp = 5인 상태로 저장 

2. BuffType - 버프 효과 (= 적용 시간인 effectiveTime 동안만 적용) 
 - 아이템 효과가 일정 시간 적용되다가(아이템 마다 다름) 적용 시간이 다 되면 적용 되었던 효과 없어짐.)
 - 게임 데이터 저장 / 로드 대상이 아님. 적용 시간 남아 있다고 해도 저장 안됨

3. PassiveType - 패시브 효과
 - 아이템 효과가 게임이 끝나기 전까지 계속 적용된다. 
 - 게임 데이터 저장 로드 적용 대상.


 -----------------------------------------------------------------------------------------


ItemUseEffect이란

아이템 효과 상세 내용으로 주로 적용 대상(playerData, weapon)에 따라서 구분 됨.


1. PlayerTargetEffect - player 정보 대상

# 플레이어 적용 효과들 구현할 만한 것들
 1. 회복류
  - hp, 허기

 2. 일정시간 상승류
  - 운, 이동속도, 공격력

 ------- 회의, 고민 필요한 것들 -------
 3. 몬스터 공격력 덜 들어오는 방어력? 쉴드 개념
    
 4. 기타
  - 9층 구멍에 캐릭터가 닿았을 때 8층으로 떨어지지 않는다.
  - 9층 구멍에 캐릭터가 닿았을 때 8층으로 떨어져도 데미지를
       입지 않는다
 
 맵에 있는 구멍 같은 것 떨어졌을 때 막는 다던가 그런 특수 한 케이스는 따로 클래스를 하나 더 만들던지
  * player 안의 bool 값들 만들어 놓고 true / false 만 해줘도 될 듯.


	# 캐릭터 관련 효과 만들만한 것

	1. 이동속도 상승

2. WeaponTargetEffect - 무기 대상 적용 효과



    무기 대상 효과 중 효과가 적용되는 타이밍(class 기준?) 일단 3곳

    1. Weapon 예시
     - 차징 시간 감소
     - 공격 속도 상승(=공격 후딜레이 감소)

    2. Pattern 예시
     - 집탄률 상승
	 - 샷건 집탄률 상승
	 - 샷건류 발사 총알 수 증가

    3. Bullet 예시
	  단순 수치 상승류
	 - 공격력 상승
     - 치명타 확률 상승
     
	  
	  on / off류
     - 비관통 총알 => 관통 총알
     - 공격시 흡혈
     - 근접 무기 상대 총알 튕겨내기 추가



	 ----------------

	
