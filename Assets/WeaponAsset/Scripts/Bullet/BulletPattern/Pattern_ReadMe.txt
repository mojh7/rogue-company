# 매우 조심.

1. CreateBullet에서 bullet을 생성하고 Init 함수를 쓸 때 bulletInfo 자체를 넣으면 안되고
bulletInfo.Clone()을 넣어줘야 데이터가 안 겹침.
 - shape Pattern 만들 때 이걸로 시간 많이 잡아 먹음.