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

Viies Ülesanne - Augmenteeritud reaalsus.

AR effekti saavutaamiseks kasutame Google ARCore raamistikku. OpenGL ES 2.0'i kaamerapildi ja objekti joonistamise jaoks. MAUI handlerit androidi vaate ühendamiseks MAUI lehega.

ARCore tuvastab pinnad, Kasutaja saab puutuda ekraanile, et lisada ankur. Objekt joonistatakse ankru kohale. Lahendus töötab hetkel ainult androidis ja vajab kaameraluba.

ARPage sisaldab nuppe ja lahenduse elutsükklit.

ARCoreMauiView sisaldab MAUI vaadet.

ARCoreMauiViewHandler loob ja haldab natiivset vaadet

ARCoreView sisaldab ARCore sessiooni, puudutuse töötlust ja mudeli joonistamist.

On VIST välja töötatud AR loogika. Lisasin kaamera, mis hetkel inverted ja AR täielikult ei tööta. Tehtud ümber csproj fail, et oleks vähem konflikte ja ei esineks kordumisi.
Tegelemine siiamaani väga ebameeldiv, palju erroreid olnud. Kui üks asi ära parandada siis tekib mitu uut errorit juurde.
Palju probleeme oli ka füüsilisel seadmel rakenduse testimisega, kuna millegi pärast läheb ajaga kõik katki ja peab käsklustega kõik korda tegema, et uuesti testimine võimalik oleks. Vähemalt sai korda ja telefoniga testitud.

Tegelesid Oskar, Dylan ja Juhan


Kuues ülesanne - Andmete hoidmine ja ligipääs

Sätette leht salvestab sätette seisud ja teksti- ja pildifailid lokaalselt app data'sse, nii, et need on olemas ka peale rakenduse sulgemist. Lehel on kolm osa: oma sätted, tekstifaili lahter ja foto lahter. Sätted salvestatakse läbi Preferences API. Teksti- ja pildifailid salvestatakse FileSystem.AppDataDirectory alla. Pilt saab unikaalse nime kuupäevaga. Foto tegemiseks kasutasime nagu varem MediaPicker'it.
Lehe uuesti avamisel loeme andmed läbi OnAppearing'u. 
Leht toetab praegu ainult ühe samanimelist "note.txt" tekstifaili salvestamist ja vaatamist. Ja näitab ainult viimati tehtud pilti.
Lehele saab liikuda läbi "sensors" lehekülje.

Preferences ja AppDataDirectory kasutamine ei esitanud raskusi.


7. ülesanne. Võrgulahendus

REST API ja HTTPS-protokoll. ReqRes

Esialgu katsetasin DummyJSON'i, kuid soovisin ka andmete hoiustamist. Proovisin ASP.NET Core Minimal API ja SQLite abil serveri loomist. Serveri ühendamisel füüsilise Android-seadmega tekkis aga probleeme arenduskeskkonna seadistusega ning oleks ka teistele tekitanud keskkonna seadistamise vajaduse.

Otsustasin kasutada ReqRes, kuna erinevalt tavalisest test-API-st võimaldab see andmeid serverisse püsivalt salvestada.

Lõin ReqRes'is movies collection'i järgmiste väljadega:
title String
coverImage String Filmi kaanepildi URL
releaseDate String (date) Filmi väljalaskekuupäev 
completed Boolean Kas film on vaadatud

Testisin API tööd PowerShelli kaudu, kasutades GET- ja POST-päringuid. POST-päringuga lisatud andmed jäid serverisse alles ning olid hiljem GET-päringuga kättesaadavad.

Tegin ReqResApiService.cs klassi, mis võimaldab kasutada valmis meetodeid ilma HTTP-päringute koostamise või JSON-andmete töötlemiseta. Lisaks lõin Models kausta Movie.cs faili, milles on kirjeldatud filmi andmemudel ja serveri vastuste struktuur.

API-teenuses kasutasin .NET-i HttpClient klassi, mis võimaldab saata HTTP-päringuid ja vastu võtta serveri vastuseid.
Kaks peamist meetodit:
GetMoviesAsync() - Meetod saadab ReqRes serverile GET-päringu, kasutades movies collection'i aadressi. Server tagastab JSON-vormingus andmed, mis teisendatakse ReadFromJsonAsync() abil C# objektideks. Meetod tagastab List<MovieRecord> nimekirja, mis sisaldab filmide ID-sid ja andmeid. Neid saab hiljem kasutada näiteks filmide kuvamiseks.

AddMovieAsync(Movie movie, string manageKey) - Meetod võimaldab lisada serverisse uue filmi. Sisendiks on Movie objekt ja kirjutamisõigusega API võti. Enne saatmist paigutatakse filmi andmed data objekti sisse. JsonContent.Create() teisendab C# objekti JSON-vormingusse. Seejärel saadetakse serverile POST-päring ning vastus teisendatakse tagasi C# objektiks.

Mõlemad meetodid on asünkroonsed, et päringute ajal ei blokeeruks kasutajaliides.

ReqRes kasutab API võtmeid ligipääsu kontrollimiseks. Andmete lugemiseks kasutan avalikku pub_ võtit, mis on lisatud API-teenuse konfiguratsiooni. See võimaldab kasutada GetMoviesAsync() meetodit ilma täiendava seadistamiseta. Andmete lisamiseks on vajalik pro_ haldusvõti. Seda ei ole GitHubis ega rakenduse lähtekoodis, vaid antakse POST-meetodile eraldi argumendina. Post lahendus sobib vaid testimiseks, edaspidi tuleks andmete muutmiseks kasutada turvalisemat autentimist.

Vajadusel saab täiendada PUT-, PATCH- ja DELETE-meetoditega.

Üldiselt oli REST API-ga ühenduse loomine .NET MAUI-s suhteliselt lihtne tänu HttpClient API-le ning JSON-i automaatsele serialiseerimisele ja deserialiseerimisele. Tähelepanu võttis serverilahenduse valimine, ReqRes seadistamine.

Kasutatud dokumentatsioon
Microsoft – Consume a REST-based web service (.NET MAUI) https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/rest?view=net-maui-10.0
Microsoft – HttpClient https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/http-overview
ReqRes – API dokumentatsioon https://reqres.in/docs

^Tegi Quentin
Seitsmes Ülesanne - 
