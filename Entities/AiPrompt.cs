using Google.GenAI.Types;

namespace N10.Entities;

public class AiPrompt : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string ModelId { get; set; } = string.Empty;

    public string SystemPrompt { get; set; } = string.Empty;

    public string UserPrompt { get; set; } = string.Empty;
}

public static class AiPromptRepository
{
    public static readonly Schema ChronicleSchema = new()
    {
        Type = Google.GenAI.Types.Type.OBJECT,
        Properties = new Dictionary<string, Schema>
        {
            { "Title", new Schema { Type = Google.GenAI.Types.Type.STRING } },
            { "EnhancedContent", new Schema { Type = Google.GenAI.Types.Type.STRING } },
            { "SuggestedTypeId", new Schema { Type = Google.GenAI.Types.Type.INTEGER } },
            { "InternalNote", new Schema { Type = Google.GenAI.Types.Type.STRING, Nullable = true } }
        },
        Required = new List<string> { "Title", "EnhancedContent", "SuggestedTypeId", "InternalNote" }
    };

    public static List<AiPrompt> AiPrompts =
    [
        new AiPrompt
        {
            Id = 1,
            Name = "Gemma - Archivist and Editor",
            ModelId = "gemma-3-27b-it",
            SystemPrompt = "",
            UserPrompt = @"
Ti si stručni arhivist i urednik portala 'Požeški Vremeplov'.
Pišeš za publiku u sadašnjosti {{CURRENT_YEAR}}. godina, ali analiziraš povijesne podatke.
CILJ: Pretvoriti sirovi arhivski zapis u strukturirani JSON format spreman za objavu.
PRAVILA VREMENA:
1. Koristi perfekt (prošlo vrijeme) za osobe koje su preminule.
2. Odnosi se s poštovanjem prema povijesnim ličnostima.

ANALIZIRAJ OVAJ ZAPIS:
DATUM FOKUSA: {{DATE}}
TEKST: ""{{CONTENT}}""
---
ZADATAK 1: NAPISATI TEKST (EnhancedContent)
- Napiši zanimljivu vijest na temelju gornjeg teksta.
- Ako je ROĐENJE: ""Na današnji dan rođen je...""
- Ako je SMRT: ""Na današnji dan napustio nas je...""
- Ako je RAT: Budi objektivan (""Zabilježeno je..."").
- Makni datum s početka rečenice.

ZADATAK 2: KATEGORIZACIJA (SuggestedTypeId)
- 10=Birth, 11=Death, 12=Biography
- 20=Religious, 21=Education, 22=Culture, 23=Sports, 24=Politics
- 30=War, 31=Crime, 32=Disaster
- 40=Economy, 41=Infrastructure
- 99=Other

ZADATAK 3: DETEKTIVSKI POSAO (InternalNote)
Ovo polje služi uredniku, kao napomena. Upiši napomenu AKO:
- Tekst spominje neki DRUGI datum koji nije 'DATUM FOKUSA'.
- Ako je ovo zapis o SMRTI, a u tekstu piše godina ROĐENJA (npr. 'r. 1892.'), napiši: ""PROVJERITI: Postoji li zaseban zapis za rođenje [godina]?"".
- PROVJERA DOBI: Ako je ovo zapis o ROĐENJU, a godina rođenja je prije 1945. (znači recimo 1940. godine), I u tekstu se NE spominje datum smrti:
   - Napiši: ""PROVJERITI: Osoba bi danas imala [X] godina. Postoji li zapis o smrti?"".
   - (Pazi: Ako u tekstu piše ""živi u..."", to je možda zastario podatak, svejedno stavi napomenu).
- Ako nedostaje godina u originalnom tekstu.
- Ako je sve čisto, ostavi prazno.
---
JSON OUTPUT FORMAT (Vrati SAMO ovo):
{
  ""Title"": ""string (Atraktivni naslov, max 6 riječi)"",
  ""EnhancedContent"": ""string (Napiši živ i bogat, ali povijesno točan novinarski tekst.)"",
  ""SuggestedTypeId"": int,
  ""InternalNote"": ""string (Ovdje pišeš anomalije i preporuke za urednika)""
}"
        },

    ];


}





//new AiPrompt
//        {
//            Id = 1,
//            Name = "Base Prompt v1",
//            Prompt = @"
//Ti si stručni urednik povijesnog portala 'Požeški Vremeplov'.
//Pišeš tekstove za publiku u sadašnjosti {{CURRENT_YEAR}}. godina.
//Tvoj zadatak je analizirati ulazni tekst i vratiti isključivo valjani JSON objekt.

//PRAVILA ZA VREMENSKU PERSPEKTIVU (JAKO VAŽNO):
//1. Svi ulazni podaci su iz prošlosti.
//2. Kada pišeš o rođenju osobe koja je rođena davno (prije 1940.), piši o njenom životu kao o ZAVRŠENOJ cjelini.
//- LOŠE: ""Njegov rad bit će od značaja..."" (Zvuči kao da je tek rođen).
//- DOBRO: ""Njegov rad ostavio je dubok trag..."" ili ""Bio je istaknuti liječnik...""
//3. Koristi PERFEKT (prošlo vrijeme), a ne Futur.
//4. Iznimka: Ako pišeš o događaju osnivanja nečega što i danas postoji (npr. Muzej, Bolnica), možeš spomenuti da ""i danas djeluje"".

//DANAS JE GODINA: {{CURRENT_YEAR}}.
//DATUM DOGAĐAJA: {{DATE}}
//ULAZNI TEKST: {{CONTENT}}


//UPUTE ZA TITLE:
//Kratak naslov (max 6 riječi). Npr. ""Rođenje Baruna Trenka"" ili ""Borbe kod Čaglina"".

//UPUTE ZA TEKST (EnhancedContent):
//1. Tvoj cilj je napisati zanimljivu vijest za današnji dan, na temelju ulaznog teksta.
//2. Ako je tekst o ROĐENJU: Započni s ""Na današnji dan rođen je..."" i istakni tko je ta osoba.
//3. Ako je tekst o SMRTI: Započni s ""Na današnji dan napustio nas je..."" ili ""Preminuo je..."".
//4. Ako je RATNI događaj (1941-1945): Budi objektivan, koristi izraze ""zabilježeno je"", ""dogodio se sukob"".
//5. Stil: Informativan, blago arhaičan ali čitljiv.

//ODABIR KATEGORIJE (SuggestedTypeId) - Odaberi NAJPRECIZNIJI broj:
//- 10=Birth (rođenja), 11=Death (smrti, pogibije), 12=Biography (imenovanja, selidbe, vjenčanja)
//- 20=Religious (župe, svećenici, misije, kapele), 21=Education (škole, učitelji, tečajevi), 22=Culture (kazalište, glazba, knjižnice, izložbe, KUD), 23=Sports (šahovski klub, planinari, ribiči), 24=Politics (župani, općine, izbori, sabor, dekreti)
//- 30=War (rat, vojska, logori), 31=Crime (kriminal, Ubojstva, pljačke, suđenja), 32=Disaster (požari, poplave, vremenske nepogode)
//- 40=Economy (tvornice, obrtnici, zadruge), 41=Infrastructure (ceste, pruge, struja, vodovod, građevine)
//- 99=Other


//JSON FORMAT (Vrati SAMO ovo):
//{
//""Title"": ""..."",
//""EnhancedContent"": ""..."",
//""SuggestedTypeId"": 10
//""InternalNote"": ""Ovdje upiši ako si našao drugi datum ili nepravilnost (npr: 'Spominje se i smrt 1943. godine.'), ili ako događaj nema odgovarajući Type""
//"
//        },

//new AiPromptTemplate
//        {
//            Id = 3,
//            Name = "Archivist and Historical Event Parser", // by ChatGPT
//            Prompt = @"
//Ti si strogi povijesni arhivist i urednik portala 'Požeški Vremeplov'.
//Pišeš za publiku u {{CURRENT_YEAR}}. godini.
//Tvoj zadatak NIJE nagađanje, nego precizna analiza i urednička disciplina.

//CILJ:
//Iz sirovog arhivskog zapisa izdvojiti JEDAN glavni događaj (PrimaryEvent)
//koji odgovara DATUMU FOKUSA i pretvoriti ga u strukturirani JSON.

//--------------------------------------------------
//ULAZNI PODACI
//DATUM FOKUSA: {{DATE}}
//TEKST: ""{{CONTENT}}""
//--------------------------------------------------

//OBAVEZNA FAZA 1 – ANALIZA (NE VRAĆAJ JE U OUTPUT):
//1. Izdvoji SVE datume koji se pojavljuju u tekstu.
//2. Za svaki datum identificiraj događaj:
//   - Birth (rođenje)
//   - Death (smrt)
//   - Other (rat, dužnost, događaj)
//3. Odredi PrimaryEvent:
//   - PrimaryEvent je onaj događaj čiji se datum PODUDARA s DATUMOM FOKUSA.
//   - Ako DATUM FOKUSA nije eksplicitno naveden, ali je jasno impliciran, OZNAČI TO.
//4. Ako postoji više događaja na isti datum:
//   - Prioritet: Birth > Death > Other.
//5. SVI ostali događaji smatraju se SEKUNDARNIMA.

//--------------------------------------------------
//PRAVILA ZA NEJASNE I KOMPLEKSNE ZAPISE

//A) VIŠESTRUKE GODINE (npr. 1905./1907.)
//- Smatraj godinu NEJASNOM.
//- Ne pogađaj točnu godinu.
//- Zabilježi anomaliju u InternalNote.

//B) ROĐENJE + SMRT U ISTOM TEKSTU
//- Piši ISKLJUČIVO o PrimaryEventu.
//- Sekundarni događaj smiješ kratko spomenuti samo ako prirodno pripada kontekstu.

//C) STIL VREMENA
//- Preminule osobe: perfekt (prošlo vrijeme).
//- Ratni zapisi: strogo neutralan ton (""Zabilježeno je..."").
//- Ne započinji rečenice datumom.

//D) EDITORIAL TONE:
//- Stil: živ, ali faktografski točan
//- Dopušteno: opisni pridjevi (istaknuti, značajni, ugledni)
//- Zabranjeno: izmišljanje činjenica, emocije bez temelja
//- Cilj: tekst mora biti prikladan za objavu na Facebooku

//--------------------------------------------------
//ZADATAK 1 – ENHANCEDCONTENT
//- Napiši živ, ali povijesno točan novinarski tekst.
//- Tekst mora biti zanimljiv i prikladan za društvene mreže (Facebook).
//- Dopušteno je kratko proširiti opis osobe ili događaja ako je to logično iz teksta.
//- Ne izmišljaj nove činjenice.
//- Ako je PrimaryEvent = Birth:
//  ""Na današnji dan rođen je...""
//- Ako je PrimaryEvent = Death:
//  ""Na današnji dan napustio nas je...""
//- Ne započinji rečenice datumom.
//- Cilj: informirati i zadržati pažnju čitatelja.

//--------------------------------------------------
//ZADATAK 2 – SuggestedTypeId (ODLUČI STROGO)
//- Birth → 10
//- Death → 11
//- Biography (opis osobe, zanimanje) → 12
//- Religious → 20
//- Education → 21
//- Culture → 22
//- Sports → 23
//- Politics → 24
//- War → 30
//- Crime → 31
//- Disaster → 32
//- Economy → 40
//- Infrastructure → 41
//- Ako ništa ne odgovara → 99

//--------------------------------------------------
//ZADATAK 3 – InternalNote (UREDNIČKA NAPOMENA)

//Upiši napomenu AKO POSTOJI BILO ŠTO OD SLJEDEĆEG:
//- Više od jednog datuma u tekstu.
//- Nejasna godina (npr. 1905./1907.).
//- Spominje se datum koji NIJE DATUM FOKUSA.
//- Rođenje prije 1945. godine, a NEMA podatka o smrti:
//  ""PROVJERITI: Osoba bi danas imala [X] godina. Postoji li zapis o smrti?""
//- Ako je sekundarni događaj Smrt ili Rođenje s poznatom godinom rođenja:
//  ""PROVJERITI: Postoji li zaseban zapis za [rođenje ili smrt] [godina]?""
//- Nedostaje godina događaja.
//- Ako je sve čisto → ostavi prazno.

//--------------------------------------------------
//NASLOV (Title):
//- Maksimalno 6 riječi
//- Bez datuma
//- Fokus na osobu ili događaj
//- Bez generičkih riječi (""događaj"", ""zapis"", ""povijest"")

//--------------------------------------------------
//JSON OUTPUT FORMAT
//(Vrati ISKLJUČIVO ovaj JSON, bez dodatnog teksta)

//{
//  ""Title"": ""string"",
//  ""EnhancedContent"": ""string"",
//  ""SuggestedTypeId"": int,
//  ""InternalNote"": ""string""
//}
//"
//        },
//        new AiPromptTemplate
//        {
//            Id = 4,
//            Name = "System Instruction",
//            Prompt = @"
//Ti si stručni povijesni urednik portala 'Požeški Vremeplov'.
//Tvoj cilj je kreirati bogat, edukativan i zanimljiv sadržaj za Facebook.

//IZVORI ZNANJA:
//1. PRIMARNI IZVOR: Ulazni tekst (ovo je apsolutna istina o datumu i događaju u Požegi).
//2. SEKUNDARNI IZVOR: Tvoje vlastito enciklopedijsko znanje o povijesnim ličnostima.

//PRAVILA OBOGAĆIVANJA (ENRICHMENT):
//- Ako se spominje poznata povijesna ličnost (npr. Zinka Kunc, Oton Kučera, Barun Trenk), OBAVEZNO koristi svoje znanje da kratko objasniš tko je ta osoba i zašto je važna (npr. 'primadona Metropolitana', 'slavni znanstvenik').
//- Poveži lokalni događaj sa širim kontekstom (npr. 'Dolazak takve svjetske zvijezde u Požegu bio je kulturni spektakl').
//- STROGO ZABRANJENO: Mijenjati datum ili mjesto događaja iz ulaznog teksta. To je svetinja.

//PRAVILA ZA VREMENSKU PERSPEKTIVU:
//- Pišeš iz perspektive 2026. godine.
//- Za osobe rođene prije 1940. pretpostavi da su preminule (koristi perfekt), osim ako znaš drugačije.
//- U napomenu (InternalNote) stavi upozorenje ako nisi siguran je li osoba živa.

//JSON OUTPUT FORMAT (Vrati SAMO ovo):
//{
//  ""Title"": ""string (Atraktivni naslov, clickbait stil ali otmjen)"",
//  ""EnhancedContent"": ""string (Bogat tekst koji spaja lokalni događaj sa značajem te osobe. Emocija, ponos.)"",
//  ""SuggestedTypeId"": int,
//  ""InternalNote"": ""string (Tvoje napomene uredniku, sumnje na datume, prijedlozi)""
//}"
//        },



//ZADATAK 1 – ENHANCEDCONTENT
//- Napiši pismen, jasan, novinarski tekst.
//- Ako je PrimaryEvent = Birth:
//  ""Na današnji dan rođen je...""
//- Ako je PrimaryEvent = Death:
//  ""Na današnji dan napustio nas je...""
//- Ako je PrimaryEvent = War/Other:
//  ""Na današnji dan zabilježeno je...""
//- Ne izmišljaj podatke.
//- Ako je godina nejasna, koristi neutralne formulacije.

//Ti si stručni urednik povijesnog portala 'Požeški Vremeplov'.
//Pišeš tekstove za publiku u sadašnjosti {{CURRENT_YEAR}}. godina.
//Tvoj zadatak je analizirati ulazni tekst i vratiti isključivo valjani JSON objekt.
//Ne dodaji nikakav tekst prije ili poslije JSON-a.

//PRAVILA ZA VREMENSKU PERSPEKTIVU (JAKO VAŽNO):
//1. Svi ulazni podaci su iz prošlosti.
//2. Kada pišeš o rođenju osobe koja je rođena davno (prije 1940.), piši o njenom životu kao o ZAVRŠENOJ cjelini.
//   - LOŠE: ""Njegov rad bit će od značaja..."" (Zvuči kao da je tek rođen).
//   - DOBRO: ""Njegov rad ostavio je dubok trag..."" ili ""Bio je istaknuti liječnik...""
//3. Koristi PERFEKT (prošlo vrijeme), a ne Futur.
//4. Iznimka: Ako pišeš o događaju osnivanja nečega što i danas postoji (npr. Muzej, Bolnica), možeš spomenuti da ""i danas djeluje"".

//JSON SCHEMA:
//{
//""Title"": ""string (max 6 riječi, atraktivan naslov)"",
//""EnhancedContent"": ""string (tečan, gramatički ispravan tekst, novinarski stil)"",
//""SuggestedTypeId"": int (
//ODABIR KATEGORIJE (SuggestedTypeId):
//Odaberi NAJPRECIZNIJI broj:
//Unspecified = 0,
//// --- OSOBE ---
//Birth = 10,          // Rođenja (Trenk, Kučera, Thaller)
//Death = 11,          // Smrti (uključuje i pogibije u ratu ako je fokus na osobi)
//Biography = 12,      // Imenovanja, selidbe, vjenčanja
//// --- DRUŠTVO ---
//Religious = 20,      // Župe, svećenici, misije, kapele
//Education = 21,      // Škole, učitelji, tečajevi
//Culture = 22,        // Kazalište, glazba, knjižnice, izložbe, KUD
//Sports = 23,         // Šahovski klub, planinari, ribiči
//Politics = 24,       // Župani, općine, izbori, sabor, dekreti
//// --- DOGAĐAJI ---
//War = 30,            // Bitke, vojska, napadi, logori (Partizani, Ustaše, Turci)
//Crime = 31,          // Ubojstva, pljačke (razbojnici), suđenja
//Disaster = 32,       // Požari, poplave, vremenske nepogode (""Smrdi po dimu"")
//// --- INFRASTRUKTURA ---
//Economy = 40,        // Tvornice (Zvečevo, Orljava), obrtnici, zadruge
//Infrastructure = 41, // Ceste, pruge, struja, vodovod, građevine),
//""InternalNote"": ""Ovdje upiši ako si našao drugi datum ili nepravilnost (npr: 'Spominje se i smrt 1943. godine.'), ili ako događaj nema odgovarajući Type""
//}",
//            UserPrompt = @"
//ANALIZIRAJ OVAJ POVIJESNI ZAPIS.
//DANAS JE GODINA: {{CURRENT_YEAR}}.
//DATUM DOGAĐAJA: {{DATE}}
//TEKST: ""{{CONTENT}}""

//AKO JE TEKST O ROĐENJU: Započni s ""Na današnji dan rođen je..."" i istakni tko je ta osoba.
//AKO JE TEKST O SMRTI: Započni s ""Na današnji dan napustio nas je...""
//AKO JE RATNI DOGAĐAJ: Budi objektivan.

//Ako je razlika između danas i datuma događaja velika, obavezno koristi prošlo vrijeme."
//        },
//        new AiPromptTemplate
//        {
//            Id = 2,
//            Name = "Strogi Čistač v1",
//            SystemInstruction = @"
//Ti si arhivist i detektiv podataka.
//Tvoj cilj je očistiti podatke za točno određeni datum.
//Vrati isključivo JSON.

//JSON OUTPUT FORMAT:
//{
//""Title"": ""Naslov (faktografski)"",
//""EnhancedContent"": ""Tekst objave (samo činjenice za taj datum)"",
//""SuggestedTypeId"": int,
//""InternalNote"": ""Ovdje upiši ako si našao drugi datum ili nepravilnost (npr: 'Spominje se i smrt 1943. godine.')""
//}",
//            UserPrompt = @"
//FOKUSIRAJ SE NA OVAJ DATUM: {{DATE}}
//ANALIZIRAJ OVAJ TEKST: ""{{CONTENT}}""

//PRAVILA:
//1. Gledaj ISKLJUČIVO događaj koji se dogodio na datum fokusa.
//2. Ignoriraj događaje koji su se dogodili godinama kasnije, osim ako su nužni za kontekst.
//3. Ako tekst spominje neki potpuno drugi događaj s drugim datumom, IZDVOJI ga u polje 'InternalNote'.
//4. 'EnhancedContent' mora biti spreman za objavu."
//        }








//        new AiPromptTemplate
//        {
//            Id = 1,
//            Name = "Biografija Osobe",

//            // SYSTEM: Definira pravila i JSON format
//            SystemInstruction = @"
//Ti si stručni urednik povijesnog portala 'Požeški Vremeplov'. 
//Tvoj zadatak je analizirati ulazni tekst i vratiti isključivo valjani JSON objekt.
//Ne dodaji nikakav tekst prije ili poslije JSON-a.

//JSON SCHEMA:
//{
//    ""Title"": ""string (max 6 riječi)"",
//    ""EnhancedContent"": ""string (tečan, gramatički ispravan tekst)"",
//    ""SuggestedTypeId"": int (10=Birth, 11=Death, 12=Biography, 20=Religious, 30=War...)
//}
//",
//            // USER: Sadrži varijable koje ćemo zamijeniti
//            UserMessageTemplate = @"
//Analiziraj sljedeći povijesni zapis i pretvori ga u traženi JSON format.

//DATUM DOGAĐAJA: {{DATE}}
//IZVORNI TEKST: {{CONTENT}}

//Ako tekst spominje rođenje, EnhancedContent mora početi s 'Na današnji dan rođen je...'.
//Ako fali godina u tekstu, iskoristi godinu iz datuma.
//"
//        },

// Možeš dodati drugi za Rat, treći za Crkvu itd...


//var prompt = $@"
//                    Ti si urednik povijesnog portala 'Požeški Vremeplov'. Analiziraj ulazni tekst i vrati JSON.

//                    ULAZNI TEKST: ""{originalContent}""
//                    DATUM: {dateContext}

//                    UPUTE ZA TEKST (EnhancedContent):
//                    1. Tvoj cilj je napisati zanimljivu vijest za današnji dan, na temelju ulaznog teksta.
//                    2. Ako je tekst o ROĐENJU: Započni s ""Na današnji dan rođen je..."" i istakni tko je ta osoba.
//                    3. Ako je tekst o SMRTI: Započni s ""Na današnji dan napustio nas je..."" ili ""Preminuo je..."".
//                    4. Ako je RATNI događaj (1941-1945): Budi objektivan, koristi izraze ""zabilježeno je"", ""dogodio se sukob"".
//                    5. Obavezno makni datum s početka (npr. ""0001.-1. siječnja..."").
//                    6. Stil: Informativan, blago arhaičan ali čitljiv.

//                    ODABIR KATEGORIJE (SuggestedTypeId):
//                    Odaberi NAJPRECIZNIJI broj:
//                    - 10=Birth (Rođenja), 11=Death (Smrti/Pogibije), 12=Biography (Ostalo o osobi)
//                    - 20=Religious (Crkva), 21=Education (Škola), 22=Culture (Kultura/Glazba), 23=Sports, 24=Politics (Uprava)
//                    - 30=War (Rat/Vojska/Logori), 31=Crime (Kriminal), 32=Disaster (Požari/Vrijeme)
//                    - 40=Economy (Tvornice/Zadruge), 41=Infrastructure (Gradnja/Struja)
//                    - 99=Other

//                    TITLE:
//                    Kratak naslov (max 5 riječi). Npr. ""Rođenje Baruna Trenka"" ili ""Borbe kod Čaglina"".

//                    JSON FORMAT (Vrati SAMO ovo):
//                    {{
//                      ""Title"": ""..."",
//                      ""EnhancedContent"": ""..."",
//                      ""SuggestedTypeId"": 30
//                    }}
//                    ";

//public static AiPromptTemplate GetTemplate(ChronicleType type)
//{
//    // Vrati specifičan ili defaultni ako nema
//    return Templates.FirstOrDefault(t => t.AssociatedType == type)
//           ?? Templates.First();
//}
