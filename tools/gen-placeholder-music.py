#!/usr/bin/env python3
"""
Generate honest PLACEHOLDER music — soft, SEAMLESSLY-LOOPING ambient pads, fully
synthesized (stdlib `wave` only). AudioSystem.PlayMusic loops Resources/Music/<track>
and keeps the current track if one is missing, so the game is musically silent until
beds are dropped in. These are deliberately minimal: sustained chords with a slow
shimmer, no melody — atmosphere, not a soundtrack. Replace any by dropping a real
.ogg/.wav with the same name.

Seamless looping: every partial/LFO frequency is snapped to an integer number of cycles
per loop, so the waveform meets itself with no click — no crossfade needed.
Re-run any time: `python3 tools/gen-placeholder-music.py`.
"""
import wave, struct, math, hashlib, os

SR = 22050
LOOP = 12.0  # seconds
OUT = os.path.join(os.path.dirname(__file__), "..", "Assets", "Resources", "Music")

# track -> (chord freqs in Hz, brightness 0..1, pulse_cycles_per_loop)
TRACKS = {
    "Hub":     ([220.0, 261.6, 329.6], 0.5, 2),    # A minor — neutral calm
    "Combat":  ([146.8, 174.6, 220.0], 0.45, 8),   # D minor, faster pulse — tension
    "Camp":    ([130.8, 164.8, 196.0], 0.6, 1),    # C major — warm
    "Explore": ([196.0, 293.7, 392.0], 0.55, 2),   # open fifths — airy
    "Court":   ([164.8, 196.0, 246.9], 0.4, 1),    # E minor — solemn
    "Vault":   ([110.0, 155.6, 220.0], 0.45, 3),   # tritone-ish — mysterious
    "Fugue":   ([110.0, 110.6, 164.5], 0.3, 1),    # detuned drone — hollow absence
}

def snap(f):  # snap to an integer number of cycles per loop → seamless
    return round(f * LOOP) / LOOP

def render(freqs, brightness, pulse_cycles):
    n = int(SR * LOOP)
    out = [0.0] * n
    plf = snap(pulse_cycles / LOOP)          # pulse LFO (whole cycles per loop)
    shimmer = snap(1.0 / LOOP)               # one slow shimmer per loop
    for ci, base in enumerate(freqs):
        # a couple of harmonics; higher ones scaled by brightness
        partials = [(1, 1.0), (2, 0.35 * brightness), (3, 0.12 * brightness)]
        detune = snap(base + (0.4 if ci == 1 else 0.0))  # gentle chorus on the middle voice
        for h, amp in partials:
            f1 = snap(base * h); f2 = snap(detune * h)
            for i in range(n):
                t = i / SR
                lfo = 0.85 + 0.15 * math.sin(2 * math.pi * shimmer * t + ci)
                pul = 0.80 + 0.20 * (0.5 + 0.5 * math.sin(2 * math.pi * plf * t))
                s = 0.5 * (math.sin(2 * math.pi * f1 * t) + math.sin(2 * math.pi * f2 * t))
                out[i] += s * amp * lfo * pul
    return out

def normalize(samples, peak=0.5):
    m = max(1e-6, max(abs(s) for s in samples))
    return [s / m * peak for s in samples]

def write_wav(path, samples):
    samples = normalize(samples)
    with wave.open(path, "w") as w:
        w.setnchannels(1); w.setsampwidth(2); w.setframerate(SR)
        w.writeframes(b"".join(struct.pack("<h", int(max(-1, min(1, s)) * 32767)) for s in samples))

def guid_for(rel): return hashlib.md5(rel.encode()).hexdigest()

META = """fileFormatVersion: 2
guid: {guid}
AudioImporter:
  externalObjects: {{}}
  serializedVersion: 6
  defaultSettings:
    loadType: 1
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 1
    quality: 0.5
    conversionMode: 0
  platformSettingOverrides: {{}}
  forceToMono: 1
  normalize: 1
  preloadAudioData: 0
  loadInBackground: 1
  ambisonic: 0
  3D: 0
  userData:\x20
  assetBundleName:\x20
  assetBundleVariant:\x20
"""

def main():
    os.makedirs(OUT, exist_ok=True)
    for name, (freqs, bright, pulse) in TRACKS.items():
        path = os.path.join(OUT, name + ".wav")
        write_wav(path, render(freqs, bright, pulse))
        rel = "Assets/Resources/Music/" + name + ".wav"
        with open(path + ".meta", "w") as f:
            f.write(META.format(guid=guid_for(rel)))
    print(f"Generated {len(TRACKS)} seamless placeholder music loops (+ .meta) into Assets/Resources/Music/")

if __name__ == "__main__":
    main()
