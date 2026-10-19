Esimene ülesanne:

Ei tekkinud suuri probleeme vajalike asjade laadimise ja installimisega, nt .NET Maui, Android Studio jne.
Luues Maui malli ja selle githubi üles laadides tundus olukord hea.
Suur probleem esines emulaatori tööle saamisega, (Oskaril) ei läinud Windows Hypervisor Platform käima, peale mitut korda arvuti taaskäivitamist. Lahenduseks oli BIOS-s CPU virtualisatsiooni käivitamine.
Oskaril oli probleem ka koodi käivitamisega Virtual Studios, probleem oli lihtsalt vale JDK versiooniga ja teiste failidega.

Debuggeri käivitamisega oli väga lihtne, lihtsalt pidi eelnevalt seadme valima, panna programmi sinna ja siis debuggerit käivitada Visual Studios.

Seadistamise juhisega pidi lihtsalt meenutama eelnevaid sammusid, mis oli enamasti lihtne.

Martin Toomas sai emulaatori (Visual Studios) tööle, kuid malliga oli probleem. (Error APT2265). Probleemi tekitas sellest, et Martinil oli ühes kaustas non-ASCII tähed. Selle parandamiseks pidi kausta ümber nimetama.



Siiamaani on panustanud:
Martin Toomas githubi loomisega, emulaatori üles panemisega, seadistamise juhise kirjutamisega, blogi täiendamisega.
Oskar malli loomise, 1% muudatusega ja blogi kirjutamisega.
Jonas debuggeri füüsilise käivitamisega
Juhan malli täiendamisega
Dylan malli täiendamisega, testimisega ja dokumentatsiooni kirjutamisega.




Kolmas ülesanne:

Alustasin erinevate navigeerimismeetodite ülevaatega ning otsustasin kasutada tab-navigatsiooni, kuna see tundus kõige lihtsam nii kasutaja kui ka implementatsiooni seisukohalt.
Esimese sammuna lisasin rakendusse kaks uut lehte: SecondPage ja ThirdPage. Seejärel lisasin AppShell-i alumise tab-navigatsiooni ning sidusin sinna kõik kolm peamist lehte. .NET MAUI teeb sellise navigeerimise lihtsaks, kuna põhiline navigeerimisstruktuur on võimalik kirjeldada otse AppShell.xaml failis.

Rakenduse peamised lehed on defineeritud AppShell.xaml failis ShellContent elementidena, mis moodustavad alumise tab-navigatsiooni. Alamlehti ei lisata otse navigeerimisribale, vaid need registreeritakse marsruutidena ning avatakse GoToAsync() meetodiga.

Järgmisena lisasin kolmanda lehe alla sügavamad alamlehed (3.1 ja 3.2). Nendele navigeerimiseks kasutasin Shell.Current.GoToAsync() meetodit koos vastava lehekülje marsruudiga. Kuna navigeerimine on asünkroonne tegevus, kasutasin async ja await, et rakenduse kasutajaliides ei hanguks lehtede vahetamise ajal.

Sügavamatele lehtedele navigeerides jäävad need vastava tab-i navigeerimisstacki alles. See tähendab, et kui kasutaja liigub näiteks lehelt 3 alamlehele 3.1, seejärel vahetab tab-i ja hiljem naaseb kolmandale tab-ile, võib MAUI kuvada uuesti lehe 3.1, mitte peamist lehte 3. Selle käitumise muutmiseks lisasin OnNavigated meetodisse kontrolli, mis tuvastab peamiste tab-ide vahel liikumise. Tab-i vahetamisel kutsutakse välja PopToRootAsync(), mis eemaldab vastava tab-i stackist sügavamad lehed ning taastab algse peamise lehe.

Mitmelehelise lahenduse ja tab-navigatsiooni implementeerimine osutus .NET MAUI-s üsna lihtsaks. Kõige rohkem tähelepanu võttis alamlehtede navigeerimisstacki käitumise mõistmine, kuna MAUI säilitab iga tab-i navigeerimisajaloo eraldi. Pärast stacki tööpõhimõtte mõistmist oli võimalik soovitud käitumine suhteliselt vähese lisakoodiga tekitada.

Panustas Quentin




Neljas ülesanne- sensorite kasutamine:

Esiteks muutsin AndroidManifest.xml faili, et deklareerida soov kasutada kaamerat. Androidis on see vajalik enne, kui rakendus saab kasutajalt kaamera kasutamiseks permissionit küsida.
Seejärel lisasin helperi sensorite (ja muude õiguste) kontrollimiseks ning küsimiseks. Helper kontrollib esmalt, kas vastav permission on juba antud, ning vajadusel küsib kasutajalt ligipääsu. Seda lahendust saab hiljem kasutada ka näiteks mikrofoni või asukoha puhul.
Lehele 3.1 lisasin nupu, mille vajutamisel kontrollitakse kaamera permissionit, vajadusel küsitakse seda ning avatakse seadme kaamera. Pärast pildi tegemist tagastab MediaPicker pildi failiviitena. Pilt kopeeritakse rakenduse cache-kausta, mille järel kuvatakse see ajutiselt lehel.
Lahendust testisin füüsilisel Android-seadmel.

Permissionite küsimine ei olnud keeruline, kuna selleks on olemas Permissions API. Microsofti dokumentatsioon:
https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/appmodel/permissions?view=net-maui-10.0&tabs=windows

Kaamera kasutamine oli samuti lihtne tänu MediaPicker API-le. Microsofti dokumentatsioon:
https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device-media/picker?view=net-maui-10.0&tabs=windows

Panustas Quentin
