# SFX Candidate Report

조사 기준: 현재 작업공간에 `AudioSourcePacks` 폴더가 없어, 실제로 존재하는 `Assets/ExternalAssets/Sound`의 무료 효과음 팩을 대상으로 후보를 선별했습니다. 원본 파일은 삭제/변경하지 않았고, `Assets`로 복사하거나 코드에 연결하지 않았습니다.

## 조사 요약

- 조사 폴더: `Assets/ExternalAssets/Sound`
- 오디오 파일 수: 285개
- 후보 선정 기준: 짧은 길이, 반복 전투 피로도, 다크 판타지/금속/마법 질감, 파일명과 팩 성격
- 음량 정보: WAV는 RMS dBFS 측정, OGG는 현재 로컬 디코더 부재로 `측정 불가` 표시

## 라이선스 정리

| 팩 | 확인 문서 | 판정 | 메모 |
|---|---|---|---|
| 80-CC0-RPG-SFX | 로컬 LICENSE 없음 | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 최종 적용 전 원출처 확인 권장 |
| kenney_impact-sounds | License.txt 확인 | CC0 | 출처 표기 불필요, Kenney 크레딧은 선택 |
| kenney_ui-audio | License.txt 확인 | CC0 | 출처 표기 불필요, Kenney 크레딧은 선택 |
| sword - StarNinjas | 로컬 LICENSE 없음 | 확인 필요 | 최종 적용 전 원출처/라이선스 확인 필요 |
| sword_clash_-_starninjas | 로컬 LICENSE 없음 | 확인 필요 | 최종 적용 전 원출처/라이선스 확인 필요 |
| shield | 로컬 LICENSE 없음 | 확인 필요 | 최종 적용 전 원출처/라이선스 확인 필요 |

## 최종 효과음별 후보

### kael_sword_wave.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.6.ogg` | sword - StarNinjas | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) | 확인 필요 | 묵직한 검기 발사음 후보. 카엘의 대검 공격에 잘 맞지만 라이선스 확인 필요. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/blade_03.ogg` | 80-CC0-RPG-SFX | 0.40s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 짧고 선명한 블레이드 계열이라 반복 공격 피로도가 낮음. |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.10.ogg` | sword - StarNinjas | 1.14s | 2 | 44100 | 측정 불가(디코더 없음) | 확인 필요 | 강한 베기감이 있어 카엘 전용 기본 공격의 차별화 후보. |

### selene_dagger_throw.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/blade_01.ogg` | 80-CC0-RPG-SFX | 0.29s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 짧고 가벼운 칼날음이라 빠른 단검 투척에 어울림. |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.1.ogg` | sword - StarNinjas | 0.97s | 2 | 44100 | 측정 불가(디코더 없음) | 확인 필요 | 얇은 베기/투척 느낌 후보. 라이선스 확인 필요. |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.2.ogg` | sword - StarNinjas | 1.08s | 2 | 44100 | 측정 불가(디코더 없음) | 확인 필요 | 짧은 날붙이 발사감 후보. 셀레네의 빠른 연사와 잘 맞음. |

### enemy_hit.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_hurt_01.ogg` | 80-CC0-RPG-SFX | 0.62s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 몬스터 피격 반응이 짧고 명확함. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_hurt_02.ogg` | 80-CC0-RPG-SFX | 0.68s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 첫 후보보다 약간 다른 질감이라 랜덤 변주용으로 좋음. |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_medium_000.ogg` | kenney_impact-sounds | 0.12s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | CC0 확인 완료, 반복 피격음으로 무난한 충격감. |

### enemy_death.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_die_01.ogg` | 80-CC0-RPG-SFX | 1.06s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 적 사망을 가장 직접적으로 표현하는 짧은 몬스터 사운드. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_monster_04.ogg` | 80-CC0-RPG-SFX | 0.72s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 일반 적보다 강한 적 사망 변주 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_misc_07.ogg` | 80-CC0-RPG-SFX | 0.35s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 덜 과한 몬스터 소멸음 후보. |

### player_hit.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_medium_001.ogg` | kenney_impact-sounds | 0.18s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | CC0 확인 완료, 플레이어 피격에 부담 없는 짧은 충격음. |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGeneric_light_002.ogg` | kenney_impact-sounds | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | 가볍고 명확해서 연속 피격에도 피로감이 적음. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_hurt_02.ogg` | 80-CC0-RPG-SFX | 0.68s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 캐릭터 피격을 더 판타지스럽게 표현할 때 쓸 후보. |

### experience_pickup.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_gem_02.ogg` | 80-CC0-RPG-SFX | 0.20s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 경험치 보석 획득감이 가장 직접적임. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_gem_01.ogg` | 80-CC0-RPG-SFX | 0.25s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 짧고 밝아서 다량 획득 시 피로도가 낮음. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_gem_03.ogg` | 80-CC0-RPG-SFX | 0.29s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 조금 더 반짝이는 보상감 후보. |

### level_up.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_02.ogg` | 80-CC0-RPG-SFX | 0.55s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 마법적 상승감이 있어 레벨업에 잘 맞음. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_misc_06.ogg` | 80-CC0-RPG-SFX | 0.72s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 보상 획득 느낌이 강한 대체 후보. |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch32.ogg` | kenney_ui-audio | 0.44s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | CC0 확인 완료, UI와 함께 쓰기 좋은 상승 피드백 후보. |

### upgrade_select.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/click3.ogg` | kenney_ui-audio | 0.09s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | CC0 확인 완료, 선택 버튼용으로 짧고 깔끔함. |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch23.ogg` | kenney_ui-audio | 0.38s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | 강화 선택에 약간 더 결정감이 있음. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_misc_03.ogg` | 80-CC0-RPG-SFX | 0.34s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 판타지 아이템 선택 느낌을 줄 수 있음. |

### game_over.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_roar_01.ogg` | 80-CC0-RPG-SFX | 0.64s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 어둡고 무거운 실패 연출 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_die_01.ogg` | 80-CC0-RPG-SFX | 1.06s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 짧게 끝나는 게임오버 전환음 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/misc_03.ogg` | 80-CC0-RPG-SFX | 0.24s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 과하지 않은 종료 피드백 후보. |

### shield_block.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/shield/magicshield_block.wav` | shield | - | - | - | 측정 실패 | 확인 필요 | 보호막 방어 성공을 가장 직접적으로 표현함. 라이선스 확인 필요. |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_medium_000.ogg` | kenney_impact-sounds | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | CC0 확인 완료, 에너지 막에 부딪히는 유리질 충격감. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/metal_02.ogg` | 80-CC0-RPG-SFX | 0.56s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 금속성 방어막 느낌을 원할 때 후보. |

### shield_ready.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_01.ogg` | 80-CC0-RPG-SFX | 0.63s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 짧은 마법 발동음으로 반복 사용에 무난함. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_02.ogg` | 80-CC0-RPG-SFX | 0.55s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 조금 더 존재감 있는 보호막 준비음 후보. |

### shield_break.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/shield/magicshield_down.wav` | shield | - | - | - | 측정 실패 | 확인 필요 | 보호막 해제/파괴 상황과 이름이 정확히 맞음. 라이선스 확인 필요. |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_heavy_001.ogg` | kenney_impact-sounds | 0.43s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | CC0 확인 완료, 깨지는 방어막 느낌이 좋음. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/metal_03.ogg` | 80-CC0-RPG-SFX | 0.43s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 단단한 막이 깨지는 느낌의 대체 후보. |

### skill_explosion.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_06.ogg` | 80-CC0-RPG-SFX | 1.00s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 화염/폭발 계열 광역기에 적합함. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_07.ogg` | 80-CC0-RPG-SFX | 0.65s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 더 강한 폭발감이 필요한 특수 강화 후보. |

### skill_ricochet.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_light_002.ogg` | kenney_impact-sounds | 0.24s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | CC0 확인 완료, 튕기는 금속성 피드백에 적합함. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/metal_01.ogg` | 80-CC0-RPG-SFX | 0.58s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 도탄의 날카로운 충돌감 후보. |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.3.ogg` | sword_clash_-_starninjas | 0.93s | 2 | 44100 | 측정 불가(디코더 없음) | 확인 필요 | 검날이 튕기는 느낌 후보. 라이선스 확인 필요. |

### skill_scatter.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/blade_02.ogg` | 80-CC0-RPG-SFX | 0.31s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 여러 발 산탄의 짧은 발사음 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_01.ogg` | 80-CC0-RPG-SFX | 0.63s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 마법 산탄 느낌으로 무난함. |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.4.ogg` | sword - StarNinjas | 0.58s | 2 | 44100 | 측정 불가(디코더 없음) | 확인 필요 | 날붙이 산탄으로 갈 때 후보. 라이선스 확인 필요. |

### skill_orbit_blade.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.8.ogg` | sword - StarNinjas | 0.96s | 2 | 44100 | 측정 불가(디코더 없음) | 확인 필요 | 회전 칼날의 지속 베기감 후보. 라이선스 확인 필요. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/blade_03.ogg` | 80-CC0-RPG-SFX | 0.40s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 짧은 베기 반복음으로 피로도가 낮음. |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.6.ogg` | sword_clash_-_starninjas | 1.32s | 2 | 44100 | 측정 불가(디코더 없음) | 확인 필요 | 적중 시 금속성 베기 피드백 후보. 라이선스 확인 필요. |

### skill_shockwave.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_05.ogg` | 80-CC0-RPG-SFX | 1.24s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 퍼지는 마법 충격파 느낌 후보. |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMining_004.ogg` | kenney_impact-sounds | 0.83s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | CC0 확인 완료, 지면 충격파 느낌이 있음. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/stones_04.ogg` | 80-CC0-RPG-SFX | 0.38s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 바닥 충격과 파편감을 줄 때 후보. |

### skill_frost.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_light_003.ogg` | kenney_impact-sounds | 0.21s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | CC0 확인 완료, 얼음 깨짐/냉기 피격감 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_02.ogg` | 80-CC0-RPG-SFX | 0.55s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 냉기 마법 발동음으로 쓸 수 있는 후보. |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_medium_002.ogg` | kenney_impact-sounds | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | 조금 더 강한 얼음 충돌 후보. |

### skill_vampirism.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_02.ogg` | 80-CC0-RPG-SFX | 1.07s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 흡혈 회복 발동의 마법적 피드백 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_misc_05.ogg` | 80-CC0-RPG-SFX | 0.60s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 어두운 생명력 흡수 느낌 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_misc_05.ogg` | 80-CC0-RPG-SFX | 0.39s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 처치 후 회복 보상음으로 덜 부담스러움. |

### boss_appear.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_roar_03.ogg` | 80-CC0-RPG-SFX | 1.16s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 보스 등장 위협감이 가장 큼. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_roar_02.ogg` | 80-CC0-RPG-SFX | 1.04s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 조금 짧고 덜 과한 보스 등장 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_monster_03.ogg` | 80-CC0-RPG-SFX | 0.99s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 일반 몬스터보다 큰 존재감 후보. |

### boss_dash.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.9.ogg` | sword - StarNinjas | 1.38s | 2 | 44100 | 측정 불가(디코더 없음) | 확인 필요 | 돌진 시작의 베기/질주감 후보. 라이선스 확인 필요. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/blade_03.ogg` | 80-CC0-RPG-SFX | 0.40s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 짧은 돌진 베기음으로 반복 사용 가능. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/chain_03.ogg` | 80-CC0-RPG-SFX | 0.93s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 무거운 보스 돌진의 쇳소리 후보. |

### boss_zone.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_04.ogg` | 80-CC0-RPG-SFX | 1.59s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 장판 생성/유지 시작음으로 명확함. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_06.ogg` | 80-CC0-RPG-SFX | 1.00s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 강한 보스 장판 생성 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_02.ogg` | 80-CC0-RPG-SFX | 0.55s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 비화염 마법진 장판에도 무난함. |

### boss_projectile.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_01.ogg` | 80-CC0-RPG-SFX | 1.06s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 보스 마법탄 발사음으로 짧고 명확함. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_03.ogg` | 80-CC0-RPG-SFX | 1.93s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 화염탄/탄막 계열에 더 잘 맞는 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_01.ogg` | 80-CC0-RPG-SFX | 0.63s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 비전 투사체 발사용 후보. |

### boss_teleport.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_02.ogg` | 80-CC0-RPG-SFX | 0.55s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 짧은 마법 이동/소환 느낌 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/misc_02.ogg` | 80-CC0-RPG-SFX | 0.54s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 순간이동 전환음으로 과하지 않음. |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch36.ogg` | kenney_ui-audio | 0.31s | 2 | 44100 | 측정 불가(디코더 없음) | CC0 | CC0 확인 완료, 순간 전환 피드백 후보. |

### boss_death.wav

| 후보 | 팩 | 길이 | 채널 | 샘플레이트 | 음량 | 라이선스 | 이유 |
|---|---|---:|---:|---:|---|---|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_die_01.ogg` | 80-CC0-RPG-SFX | 1.06s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 보스 사망 기본 후보. 짧게 처리 가능. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_roar_01.ogg` | 80-CC0-RPG-SFX | 0.64s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 더 비극적이고 무거운 보스 사망 후보. |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_monster_04.ogg` | 80-CC0-RPG-SFX | 0.72s | 2 | 48000 | 측정 불가(디코더 없음) | 팩 이름상 CC0로 보이나 로컬 문서 미확인 | 몬스터형 보스 사망 변주 후보. |

## 제외 기준

- 길이가 3초를 넘는 파일은 반복 전투 효과음 후보에서 제외했습니다.
- 발소리, 나무/돌 단순 충돌음처럼 현재 최종 목록과 직접 관련이 낮은 파일은 후보에서 제외했습니다.
- 라이선스 문서가 없는 팩은 후보로만 남기고, 실제 적용 전 확인 필요로 표시했습니다.

## 전체 파일 메타데이터

| 파일 | 팩 | 확장자 | 길이 | 채널 | 샘플레이트 | 음량 |
|---|---|---|---:|---:|---:|---|
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/blade_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.29s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/blade_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.31s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/blade_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.40s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/book_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.72s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/book_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.88s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/book_03.ogg` | 80-CC0-RPG-SFX | .ogg | 1.22s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/book_04.ogg` | 80-CC0-RPG-SFX | .ogg | 1.15s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/chain_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.51s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/chain_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.49s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/chain_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.93s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_die_01.ogg` | 80-CC0-RPG-SFX | .ogg | 1.06s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_hurt_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.62s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_hurt_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.68s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_misc_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.26s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_misc_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.19s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_misc_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.64s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_misc_04.ogg` | 80-CC0-RPG-SFX | .ogg | 0.55s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_misc_05.ogg` | 80-CC0-RPG-SFX | .ogg | 0.60s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_misc_06.ogg` | 80-CC0-RPG-SFX | .ogg | 0.42s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_misc_07.ogg` | 80-CC0-RPG-SFX | .ogg | 0.35s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_misc_08.ogg` | 80-CC0-RPG-SFX | .ogg | 0.46s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_monster_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.26s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_monster_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.24s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_monster_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.99s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_monster_04.ogg` | 80-CC0-RPG-SFX | .ogg | 0.72s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_roar_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.64s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_roar_02.ogg` | 80-CC0-RPG-SFX | .ogg | 1.04s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_roar_03.ogg` | 80-CC0-RPG-SFX | .ogg | 1.16s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_slime_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.67s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_slime_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.74s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_slime_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.48s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/creature_slime_04.ogg` | 80-CC0-RPG-SFX | .ogg | 0.62s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_coins_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.41s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_coins_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.62s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_coins_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.48s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_coins_04.ogg` | 80-CC0-RPG-SFX | .ogg | 1.61s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_gem_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.25s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_gem_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.20s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_gem_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.29s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_gem_04.ogg` | 80-CC0-RPG-SFX | .ogg | 0.59s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_misc_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.41s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_misc_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.44s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_misc_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.34s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_misc_04.ogg` | 80-CC0-RPG-SFX | .ogg | 0.38s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_misc_05.ogg` | 80-CC0-RPG-SFX | .ogg | 0.39s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_misc_06.ogg` | 80-CC0-RPG-SFX | .ogg | 0.72s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_stone_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.73s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_stone_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.30s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_stone_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.39s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_stone_04.ogg` | 80-CC0-RPG-SFX | .ogg | 0.30s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_wood_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.54s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_wood_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.26s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/item_wood_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.33s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/lock_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.41s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/lock_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.40s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/lock_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.34s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/metal_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.58s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/metal_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.56s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/metal_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.43s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/misc_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.34s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/misc_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.54s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/misc_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.24s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.63s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.55s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_01.ogg` | 80-CC0-RPG-SFX | .ogg | 1.06s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_02.ogg` | 80-CC0-RPG-SFX | .ogg | 1.07s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_03.ogg` | 80-CC0-RPG-SFX | .ogg | 1.93s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_04.ogg` | 80-CC0-RPG-SFX | .ogg | 1.59s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_05.ogg` | 80-CC0-RPG-SFX | .ogg | 1.24s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_06.ogg` | 80-CC0-RPG-SFX | .ogg | 1.00s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/spell_fire_07.ogg` | 80-CC0-RPG-SFX | .ogg | 0.65s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/stones_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.68s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/stones_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.78s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/stones_03.ogg` | 80-CC0-RPG-SFX | .ogg | 1.20s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/stones_04.ogg` | 80-CC0-RPG-SFX | .ogg | 0.38s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/wood_01.ogg` | 80-CC0-RPG-SFX | .ogg | 0.24s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/wood_02.ogg` | 80-CC0-RPG-SFX | .ogg | 0.19s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/wood_03.ogg` | 80-CC0-RPG-SFX | .ogg | 0.30s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/wood_04.ogg` | 80-CC0-RPG-SFX | .ogg | 0.65s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/80-CC0-RPG-SFX/wood_05.ogg` | 80-CC0-RPG-SFX | .ogg | 0.39s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_carpet_000.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_carpet_001.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_carpet_002.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_carpet_003.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_carpet_004.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_concrete_000.ogg` | kenney_impact-sounds | .ogg | 0.11s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_concrete_001.ogg` | kenney_impact-sounds | .ogg | 0.11s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_concrete_002.ogg` | kenney_impact-sounds | .ogg | 0.11s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_concrete_003.ogg` | kenney_impact-sounds | .ogg | 0.11s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_concrete_004.ogg` | kenney_impact-sounds | .ogg | 0.11s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_grass_000.ogg` | kenney_impact-sounds | .ogg | 0.78s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_grass_001.ogg` | kenney_impact-sounds | .ogg | 0.67s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_grass_002.ogg` | kenney_impact-sounds | .ogg | 0.69s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_grass_003.ogg` | kenney_impact-sounds | .ogg | 0.67s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_grass_004.ogg` | kenney_impact-sounds | .ogg | 0.59s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_snow_000.ogg` | kenney_impact-sounds | .ogg | 0.37s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_snow_001.ogg` | kenney_impact-sounds | .ogg | 0.37s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_snow_002.ogg` | kenney_impact-sounds | .ogg | 0.37s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_snow_003.ogg` | kenney_impact-sounds | .ogg | 0.37s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_snow_004.ogg` | kenney_impact-sounds | .ogg | 0.37s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_wood_000.ogg` | kenney_impact-sounds | .ogg | 0.25s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_wood_001.ogg` | kenney_impact-sounds | .ogg | 0.25s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_wood_002.ogg` | kenney_impact-sounds | .ogg | 0.25s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_wood_003.ogg` | kenney_impact-sounds | .ogg | 0.25s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/footstep_wood_004.ogg` | kenney_impact-sounds | .ogg | 0.25s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactBell_heavy_000.ogg` | kenney_impact-sounds | .ogg | 1.48s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactBell_heavy_001.ogg` | kenney_impact-sounds | .ogg | 1.74s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactBell_heavy_002.ogg` | kenney_impact-sounds | .ogg | 0.70s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactBell_heavy_003.ogg` | kenney_impact-sounds | .ogg | 0.65s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactBell_heavy_004.ogg` | kenney_impact-sounds | .ogg | 0.30s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGeneric_light_000.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGeneric_light_001.ogg` | kenney_impact-sounds | .ogg | 0.12s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGeneric_light_002.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGeneric_light_003.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGeneric_light_004.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_heavy_000.ogg` | kenney_impact-sounds | .ogg | 0.24s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_heavy_001.ogg` | kenney_impact-sounds | .ogg | 0.43s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_heavy_002.ogg` | kenney_impact-sounds | .ogg | 0.25s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_heavy_003.ogg` | kenney_impact-sounds | .ogg | 0.17s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_heavy_004.ogg` | kenney_impact-sounds | .ogg | 0.40s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_light_000.ogg` | kenney_impact-sounds | .ogg | 0.21s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_light_001.ogg` | kenney_impact-sounds | .ogg | 0.21s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_light_002.ogg` | kenney_impact-sounds | .ogg | 0.21s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_light_003.ogg` | kenney_impact-sounds | .ogg | 0.21s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_light_004.ogg` | kenney_impact-sounds | .ogg | 0.21s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_medium_000.ogg` | kenney_impact-sounds | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_medium_001.ogg` | kenney_impact-sounds | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_medium_002.ogg` | kenney_impact-sounds | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_medium_003.ogg` | kenney_impact-sounds | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactGlass_medium_004.ogg` | kenney_impact-sounds | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_heavy_000.ogg` | kenney_impact-sounds | .ogg | 0.17s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_heavy_001.ogg` | kenney_impact-sounds | .ogg | 0.36s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_heavy_002.ogg` | kenney_impact-sounds | .ogg | 0.12s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_heavy_003.ogg` | kenney_impact-sounds | .ogg | 0.21s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_heavy_004.ogg` | kenney_impact-sounds | .ogg | 0.13s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_light_000.ogg` | kenney_impact-sounds | .ogg | 0.35s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_light_001.ogg` | kenney_impact-sounds | .ogg | 0.25s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_light_002.ogg` | kenney_impact-sounds | .ogg | 0.24s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_light_003.ogg` | kenney_impact-sounds | .ogg | 0.48s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_light_004.ogg` | kenney_impact-sounds | .ogg | 0.21s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_medium_000.ogg` | kenney_impact-sounds | .ogg | 0.27s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_medium_001.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_medium_002.ogg` | kenney_impact-sounds | .ogg | 0.12s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_medium_003.ogg` | kenney_impact-sounds | .ogg | 0.25s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMetal_medium_004.ogg` | kenney_impact-sounds | .ogg | 0.11s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMining_000.ogg` | kenney_impact-sounds | .ogg | 0.94s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMining_001.ogg` | kenney_impact-sounds | .ogg | 0.87s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMining_002.ogg` | kenney_impact-sounds | .ogg | 0.80s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMining_003.ogg` | kenney_impact-sounds | .ogg | 0.99s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactMining_004.ogg` | kenney_impact-sounds | .ogg | 0.83s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlank_medium_000.ogg` | kenney_impact-sounds | .ogg | 0.78s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlank_medium_001.ogg` | kenney_impact-sounds | .ogg | 0.78s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlank_medium_002.ogg` | kenney_impact-sounds | .ogg | 0.78s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlank_medium_003.ogg` | kenney_impact-sounds | .ogg | 0.78s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlank_medium_004.ogg` | kenney_impact-sounds | .ogg | 0.78s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_heavy_000.ogg` | kenney_impact-sounds | .ogg | 0.49s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_heavy_001.ogg` | kenney_impact-sounds | .ogg | 0.35s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_heavy_002.ogg` | kenney_impact-sounds | .ogg | 0.49s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_heavy_003.ogg` | kenney_impact-sounds | .ogg | 0.35s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_heavy_004.ogg` | kenney_impact-sounds | .ogg | 0.56s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_light_000.ogg` | kenney_impact-sounds | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_light_001.ogg` | kenney_impact-sounds | .ogg | 0.65s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_light_002.ogg` | kenney_impact-sounds | .ogg | 0.49s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_light_003.ogg` | kenney_impact-sounds | .ogg | 0.53s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_light_004.ogg` | kenney_impact-sounds | .ogg | 0.66s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_medium_000.ogg` | kenney_impact-sounds | .ogg | 0.61s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_medium_001.ogg` | kenney_impact-sounds | .ogg | 0.62s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_medium_002.ogg` | kenney_impact-sounds | .ogg | 0.52s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_medium_003.ogg` | kenney_impact-sounds | .ogg | 0.65s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPlate_medium_004.ogg` | kenney_impact-sounds | .ogg | 0.53s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPunch_heavy_000.ogg` | kenney_impact-sounds | .ogg | 0.65s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPunch_heavy_001.ogg` | kenney_impact-sounds | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPunch_heavy_002.ogg` | kenney_impact-sounds | .ogg | 0.46s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPunch_heavy_003.ogg` | kenney_impact-sounds | .ogg | 0.47s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPunch_heavy_004.ogg` | kenney_impact-sounds | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPunch_medium_000.ogg` | kenney_impact-sounds | .ogg | 0.43s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPunch_medium_001.ogg` | kenney_impact-sounds | .ogg | 0.40s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPunch_medium_002.ogg` | kenney_impact-sounds | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPunch_medium_003.ogg` | kenney_impact-sounds | .ogg | 0.46s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactPunch_medium_004.ogg` | kenney_impact-sounds | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_heavy_000.ogg` | kenney_impact-sounds | .ogg | 0.51s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_heavy_001.ogg` | kenney_impact-sounds | .ogg | 0.57s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_heavy_002.ogg` | kenney_impact-sounds | .ogg | 0.57s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_heavy_003.ogg` | kenney_impact-sounds | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_heavy_004.ogg` | kenney_impact-sounds | .ogg | 0.50s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_medium_000.ogg` | kenney_impact-sounds | .ogg | 0.12s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_medium_001.ogg` | kenney_impact-sounds | .ogg | 0.18s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_medium_002.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_medium_003.ogg` | kenney_impact-sounds | .ogg | 0.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactSoft_medium_004.ogg` | kenney_impact-sounds | .ogg | 0.15s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactTin_medium_000.ogg` | kenney_impact-sounds | .ogg | 0.16s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactTin_medium_001.ogg` | kenney_impact-sounds | .ogg | 0.17s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactTin_medium_002.ogg` | kenney_impact-sounds | .ogg | 0.13s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactTin_medium_003.ogg` | kenney_impact-sounds | .ogg | 0.21s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactTin_medium_004.ogg` | kenney_impact-sounds | .ogg | 0.18s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_heavy_000.ogg` | kenney_impact-sounds | .ogg | 0.31s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_heavy_001.ogg` | kenney_impact-sounds | .ogg | 0.31s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_heavy_002.ogg` | kenney_impact-sounds | .ogg | 0.31s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_heavy_003.ogg` | kenney_impact-sounds | .ogg | 0.31s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_heavy_004.ogg` | kenney_impact-sounds | .ogg | 0.31s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_light_000.ogg` | kenney_impact-sounds | .ogg | 0.27s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_light_001.ogg` | kenney_impact-sounds | .ogg | 0.27s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_light_002.ogg` | kenney_impact-sounds | .ogg | 0.27s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_light_003.ogg` | kenney_impact-sounds | .ogg | 0.27s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_light_004.ogg` | kenney_impact-sounds | .ogg | 0.27s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_medium_000.ogg` | kenney_impact-sounds | .ogg | 0.33s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_medium_001.ogg` | kenney_impact-sounds | .ogg | 0.33s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_medium_002.ogg` | kenney_impact-sounds | .ogg | 0.33s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_medium_003.ogg` | kenney_impact-sounds | .ogg | 0.33s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_impact-sounds/Audio/impactWood_medium_004.ogg` | kenney_impact-sounds | .ogg | 0.33s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/click1.ogg` | kenney_ui-audio | .ogg | 0.09s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/click2.ogg` | kenney_ui-audio | .ogg | 0.06s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/click3.ogg` | kenney_ui-audio | .ogg | 0.09s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/click4.ogg` | kenney_ui-audio | .ogg | 0.04s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/click5.ogg` | kenney_ui-audio | .ogg | 0.03s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/mouseclick1.ogg` | kenney_ui-audio | .ogg | 0.06s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/mouserelease1.ogg` | kenney_ui-audio | .ogg | 0.07s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/rollover1.ogg` | kenney_ui-audio | .ogg | 0.23s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/rollover2.ogg` | kenney_ui-audio | .ogg | 0.06s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/rollover3.ogg` | kenney_ui-audio | .ogg | 0.07s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/rollover4.ogg` | kenney_ui-audio | .ogg | 0.11s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/rollover5.ogg` | kenney_ui-audio | .ogg | 0.11s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/rollover6.ogg` | kenney_ui-audio | .ogg | 0.17s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch1.ogg` | kenney_ui-audio | .ogg | 0.31s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch10.ogg` | kenney_ui-audio | .ogg | 0.37s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch11.ogg` | kenney_ui-audio | .ogg | 0.30s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch12.ogg` | kenney_ui-audio | .ogg | 0.05s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch13.ogg` | kenney_ui-audio | .ogg | 0.03s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch14.ogg` | kenney_ui-audio | .ogg | 0.03s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch15.ogg` | kenney_ui-audio | .ogg | 0.26s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch16.ogg` | kenney_ui-audio | .ogg | 0.36s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch17.ogg` | kenney_ui-audio | .ogg | 0.33s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch18.ogg` | kenney_ui-audio | .ogg | 0.44s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch19.ogg` | kenney_ui-audio | .ogg | 0.38s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch2.ogg` | kenney_ui-audio | .ogg | 0.30s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch20.ogg` | kenney_ui-audio | .ogg | 0.36s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch21.ogg` | kenney_ui-audio | .ogg | 0.41s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch22.ogg` | kenney_ui-audio | .ogg | 0.36s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch23.ogg` | kenney_ui-audio | .ogg | 0.38s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch24.ogg` | kenney_ui-audio | .ogg | 0.28s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch25.ogg` | kenney_ui-audio | .ogg | 0.38s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch26.ogg` | kenney_ui-audio | .ogg | 0.23s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch27.ogg` | kenney_ui-audio | .ogg | 0.28s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch28.ogg` | kenney_ui-audio | .ogg | 0.20s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch29.ogg` | kenney_ui-audio | .ogg | 0.31s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch3.ogg` | kenney_ui-audio | .ogg | 0.37s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch30.ogg` | kenney_ui-audio | .ogg | 0.36s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch31.ogg` | kenney_ui-audio | .ogg | 0.44s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch32.ogg` | kenney_ui-audio | .ogg | 0.44s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch33.ogg` | kenney_ui-audio | .ogg | 0.51s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch34.ogg` | kenney_ui-audio | .ogg | 0.46s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch35.ogg` | kenney_ui-audio | .ogg | 0.33s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch36.ogg` | kenney_ui-audio | .ogg | 0.31s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch37.ogg` | kenney_ui-audio | .ogg | 0.31s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch38.ogg` | kenney_ui-audio | .ogg | 0.42s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch4.ogg` | kenney_ui-audio | .ogg | 0.42s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch5.ogg` | kenney_ui-audio | .ogg | 0.31s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch6.ogg` | kenney_ui-audio | .ogg | 0.35s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch7.ogg` | kenney_ui-audio | .ogg | 0.19s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch8.ogg` | kenney_ui-audio | .ogg | 0.33s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Audio/switch9.ogg` | kenney_ui-audio | .ogg | 0.26s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/kenney_ui-audio/Preview.ogg` | kenney_ui-audio | .ogg | 14.14s | 2 | 48000 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/shield/magic-shield.wav` | shield | .wav | 4.46s | 2 | 44100 | -40.8 dBFS |
| `Assets/ExternalAssets/Sound/shield/magicshield_block.wav` | shield | .wav | - | - | - | 측정 실패 |
| `Assets/ExternalAssets/Sound/shield/magicshield_down.wav` | shield | .wav | - | - | - | 측정 실패 |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.1.ogg` | sword - StarNinjas | .ogg | 0.97s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.10.ogg` | sword - StarNinjas | .ogg | 1.14s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.2.ogg` | sword - StarNinjas | .ogg | 1.08s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.3.ogg` | sword - StarNinjas | .ogg | 0.62s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.4.ogg` | sword - StarNinjas | .ogg | 0.58s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.5.ogg` | sword - StarNinjas | .ogg | 1.06s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.6.ogg` | sword - StarNinjas | .ogg | 0.54s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.7.ogg` | sword - StarNinjas | .ogg | 0.77s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.8.ogg` | sword - StarNinjas | .ogg | 0.96s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword - StarNinjas/sword.9.ogg` | sword - StarNinjas | .ogg | 1.38s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.1.ogg` | sword_clash_-_starninjas | .ogg | 1.03s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.10.ogg` | sword_clash_-_starninjas | .ogg | 0.85s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.2.ogg` | sword_clash_-_starninjas | .ogg | 0.80s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.3.ogg` | sword_clash_-_starninjas | .ogg | 0.93s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.4.ogg` | sword_clash_-_starninjas | .ogg | 0.92s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.5.ogg` | sword_clash_-_starninjas | .ogg | 0.66s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.6.ogg` | sword_clash_-_starninjas | .ogg | 1.32s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.7.ogg` | sword_clash_-_starninjas | .ogg | 1.17s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.8.ogg` | sword_clash_-_starninjas | .ogg | 1.02s | 2 | 44100 | 측정 불가(디코더 없음) |
| `Assets/ExternalAssets/Sound/sword_clash_-_starninjas/sword_clash.9.ogg` | sword_clash_-_starninjas | .ogg | 1.09s | 2 | 44100 | 측정 불가(디코더 없음) |
