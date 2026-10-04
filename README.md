# Käivitamise juhis

## Eeldused

Projekti käivitamiseks on vajalik:

**WINDOWS:**
- Visual Studio
- Visual Studio NET Multi-platform APP UI development
- Android SDK
- Android emulator or android device

## Projekti kloonimine:

Klooni projekt:
`gh repo clone martkrip/grupp4_mobiilirakendused`

Liigu projekti kausta

`cd grupp4_mobiilirakendused`

## Seadistamine Visual Studios ja rakenduse paigaldamine seadmesse
1. Ava projekt Visual Studios
2. Ava `MauiAndroidTest` kaust
3. Tools -> Android -> Android Device Manager -> + New -> Pixel 8 - API 36.0
4. Käivita telefon
5. Vali Pixel 8 seadmena Virtual Studios
6. Build solution, et projekt kompileerida
7. Deploy solution, et seadmesse paigaldada programm
8. Käivita programm seadmes

Rakendust saab ka käivitada füüsilises Android-seadmes, kui seadmel on arendaja režiim ja USB debugging sisse lülitatud ning on ühendatud arvutiga

## Debugger käivitamine Virtual Studios
1. Vali `MauiAndroidTest` käivitamisprojektiks
2. Vali käivitatud virtuaalne või füüsiline Androidi seade
3. Käivita rohelise nupuga
