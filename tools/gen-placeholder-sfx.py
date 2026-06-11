#!/usr/bin/env python3
"""
Generate honest PLACEHOLDER sound effects for the game, fully synthesized in pure
Python (stdlib `wave` only — no samples, no licenses, no external tools).

Why this exists: AudioSystem plays Resources/SFX/<name> one-shots and no-ops when a
clip is missing, so combat is silent until audio is dropped in. This emits the exact
names the engine asks for (AudioSystem.PlaySfx / PlayHit), so they *just play* on the
next Play — then you replace any of them by dropping a real clip with the same name.

Emits a committed AudioImporter *.meta beside each .wav (stable GUID) to match the
repo's art-gen convention. Re-run any time: `python3 tools/gen-placeholder-sfx.py`.
"""
import wave, struct, math, random, hashlib, os

SR = 22050
OUT = os.path.join(os.path.dirname(__file__), "..", "Assets", "Resources", "SFX")

# ---- tiny synth toolkit ----------------------------------------------------
def silence(dur): return [0.0] * int(SR * dur)

def mix(*tracks):
    n = max(len(t) for t in tracks)
    out = [0.0] * n
    for t in tracks:
        for i, s in enumerate(t): out[i] += s
    return out

def env(samples, attack=0.005, decay=0.2, curve=4.0):
    n = len(samples); a = int(SR * attack); out = []
    for i, s in enumerate(samples):
        if i < a: g = i / max(1, a)
        else:
            x = (i - a) / max(1, (n - a))
            g = (1.0 - x) ** curve
        out.append(s * g)
    return out

def tone(freq, dur, vibrato=0.0, vib_hz=6.0, partials=(1.0,)):
    out = []
    for i in range(int(SR * dur)):
        t = i / SR
        f = freq * (1.0 + vibrato * math.sin(2 * math.pi * vib_hz * t))
        s = sum(amp * math.sin(2 * math.pi * f * h * t) for h, amp in enumerate(partials, 1))
        out.append(s)
    return out

def sweep(f0, f1, dur, partials=(1.0,)):
    out = []
    for i in range(int(SR * dur)):
        t = i / SR; frac = i / max(1, int(SR * dur))
        f = f0 * (f1 / f0) ** frac
        s = sum(amp * math.sin(2 * math.pi * f * h * t) for h, amp in enumerate(partials, 1))
        out.append(s)
    return out

def noise(dur): return [random.uniform(-1, 1) for _ in range(int(SR * dur))]

def lowpass(samples, a=0.25):
    out = []; prev = 0.0
    for s in samples:
        prev = prev + a * (s - prev); out.append(prev)
    return out

def gain(samples, g): return [s * g for s in samples]

def chord(freqs, dur, partials=(1.0, 0.4)):
    return mix(*[tone(f, dur, partials=partials) for f in freqs])

# ---- the SFX set (exact names AudioSystem requests) ------------------------
def make(name):
    random.seed(int(hashlib.md5(name.encode()).hexdigest(), 16) & 0xffffffff)
    if name in ("hit", "hit_physical"):
        return mix(env(tone(140, 0.16, partials=(1, 0.5)), decay=0.16, curve=5),
                   env(gain(lowpass(noise(0.10), 0.5), 0.5), decay=0.10, curve=6))
    if name == "crit":
        body = env(sweep(320, 120, 0.28, partials=(1, 0.4, 0.2)), decay=0.28, curve=3)
        crack = env(gain(noise(0.12), 0.6), decay=0.12, curve=5)
        return mix(body, crack)
    if name == "miss":
        return env(gain(highpass(noise(0.22)), 0.35), attack=0.02, decay=0.22, curve=2)
    if name == "heal":
        return env(mix(sweep(440, 880, 0.45, partials=(1, 0.5, 0.25)),
                       gain(sweep(660, 1320, 0.45), 0.4)), attack=0.02, decay=0.45, curve=2)
    if name == "levelup":
        notes = [523, 659, 784, 1047]; seg = 0.13; out = []
        for f in notes: out += env(tone(f, seg, partials=(1, 0.5, 0.25)), decay=seg, curve=2)
        return out
    if name == "deed":
        return env(tone(880, 0.34, partials=(1, 0.6, 0.3, 0.15)), attack=0.003, decay=0.34, curve=3)
    # elemental hit_* : a physical thud tinted by element
    base = env(tone(150, 0.12, partials=(1, 0.4)), decay=0.12, curve=5)
    if name == "hit_fire":      flav = env(gain(lowpass(noise(0.22), 0.6), 0.45), decay=0.22, curve=3)
    elif name == "hit_ice":     flav = env(tone(1800, 0.18, partials=(1, 0.5)), decay=0.18, curve=4)
    elif name == "hit_lightning": flav = env(gain(noise(0.06), 0.7), decay=0.06, curve=7)
    elif name == "hit_holy":    flav = env(chord([784, 988, 1175], 0.3), decay=0.3, curve=2)
    elif name == "hit_dark":    flav = env(mix(tone(70, 0.3), tone(73, 0.3)), decay=0.3, curve=3)
    elif name == "hit_acid":    flav = env([s * (0.5 + 0.5 * math.sin(2*math.pi*30*i/SR)) for i, s in enumerate(noise(0.25))], decay=0.25, curve=3)
    elif name == "hit_poison":  flav = env(tone(180, 0.3, vibrato=0.3, vib_hz=11), decay=0.3, curve=3)
    else: flav = silence(0.05)
    return mix(base, gain(flav, 0.6))

def highpass(samples, a=0.6):
    out = []; prev_in = 0.0; prev_out = 0.0
    for s in samples:
        o = a * (prev_out + s - prev_in); out.append(o); prev_in = s; prev_out = o
    return out

# ---- WAV + .meta -----------------------------------------------------------
def normalize(samples, peak=0.85):
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
    loadType: 0
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 1
    quality: 1
    conversionMode: 0
  platformSettingOverrides: {{}}
  forceToMono: 0
  normalize: 1
  preloadAudioData: 1
  loadInBackground: 0
  ambisonic: 0
  3D: 1
  userData:\x20
  assetBundleName:\x20
  assetBundleVariant:\x20
"""

NAMES = ["hit", "hit_physical", "hit_fire", "hit_ice", "hit_lightning", "hit_holy",
         "hit_dark", "hit_acid", "hit_poison", "crit", "miss", "heal", "deed", "levelup"]

def main():
    os.makedirs(OUT, exist_ok=True)
    for name in NAMES:
        path = os.path.join(OUT, name + ".wav")
        write_wav(path, make(name))
        rel = "Assets/Resources/SFX/" + name + ".wav"
        with open(path + ".meta", "w") as f:
            f.write(META.format(guid=guid_for(rel)))
    print(f"Generated {len(NAMES)} placeholder SFX (+ .meta) into Assets/Resources/SFX/")

if __name__ == "__main__":
    main()
