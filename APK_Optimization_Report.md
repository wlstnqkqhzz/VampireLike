# APK Optimization Report

## Baseline

- Current APK reported by user: about 70 MB
- Build Report summary reported by user:
  - Texture: about 70.5 MB
  - Sound: about 18.2 MB

## Applied

### Audio

- Target: `Assets/Resources/Music/*.ogg`
- Before:
  - BGM import quality: `1`
  - Some BGM files used non-streaming load types.
  - Some BGM files had `preloadAudioData: 1`.
- After:
  - Load Type: `Streaming`
  - Compression Format: existing Vorbis setting retained
  - Quality: `0.7`
  - Preload Audio Data: disabled
- Reason:
  - BGM was the largest safe optimization target in `Resources`.
  - Quality `0.7` is a conservative first pass intended to preserve audible quality.

### Texture

- Target: `Assets/Resources/Menu/title_background.png`
- Before:
  - No Android platform override
  - Max Size: `4096`
- After:
  - Android platform override enabled
  - Android Max Size: `2048`
  - Automatic format retained
- Reason:
  - Source image is `1672 x 941`, so this should not downscale the actual image.
  - This keeps visual quality while avoiding unnecessarily high platform max size.

### Resources cleanup

- Removed unused legacy shield WAV files:
  - `Assets/Resources/Sounds/shield_block.wav`
  - `Assets/Resources/Sounds/shield_break.wav`
  - `Assets/Resources/Sounds/shield_ready.wav`
- Reason:
  - Current code references `shield_block_sfx`, `shield_break_sfx`, and `shield_ready_sfx` instead.
  - Removed files were old large WAV versions and were still inside `Resources`.

### Android build

- Before:
  - `AndroidMinifyRelease: 0`
  - `Strip Engine Code: enabled`
  - Android target architecture: ARM64
  - Android scripting backend: IL2CPP
- After:
  - `AndroidMinifyRelease: 1`
  - Existing IL2CPP, ARM64, and Strip Engine Code settings retained
- Reason:
  - R8 release minify can reduce Java-side build size with low risk.
  - Managed stripping was not changed because the project uses dynamic loading through `Resources.Load`.

## Not Applied

- Character and boss animation sprite max size reduction:
  - Not applied to avoid blur, pixel edge loss, and animation quality loss.
- `DarkForestBackground.png` max size reduction:
  - Not applied because the in-game background is visually sensitive and still being tuned.
- Package removal:
  - Not applied. Package removal can break editor workflows or 2D import tooling and should be handled separately.
- Managed Stripping Level increase:
  - Not applied because dynamic loading patterns require Android runtime validation first.

## Validation Needed

1. Rebuild Android APK after Unity reimports the changed assets.
2. Compare APK size against the previous about 70 MB build.
3. On device, check:
   - Main menu BGM quality
   - Battle BGM quality
   - Boss BGM quality
   - Game over BGM quality
   - Main menu background sharpness
   - Shield SFX still plays correctly
4. If BGM quality is acceptable, a second pass can test quality `0.6`.
5. If BGM quality is noticeably worse, restore BGM quality from `0.7` to `1`.
