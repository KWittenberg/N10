namespace N10.Entities;

public class AiPromptTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty; // Npr. "Biografija v1"

    public string SystemInstruction { get; set; } = string.Empty; // SYSTEM PROMPT: "Ti si..." (Kontekst i pravila)

    public string UserMessageTemplate { get; set; } = string.Empty; // USER PROMPT: Špranca gdje ćemo ubaciti {{CONTENT}} i {{DATE}}
}


public static class PromptRepository
{
    public static List<AiPromptTemplate> Templates =
    [
        new AiPromptTemplate
        {
            Id = 1,
            Name = "Kreativni Urednik v1",
            SystemInstruction = @"
Ti si stručni urednik povijesnog portala 'Požeški Vremeplov'.
Pišeš tekstove za publiku u sadašnjosti {{CURRENT_YEAR}}. godina.
Tvoj zadatak je analizirati ulazni tekst i vratiti isključivo valjani JSON objekt.
Ne dodaji nikakav tekst prije ili poslije JSON-a.

PRAVILA ZA VREMENSKU PERSPEKTIVU (JAKO VAŽNO):
1. Svi ulazni podaci su iz prošlosti.
2. Kada pišeš o rođenju osobe koja je rođena davno (prije 1940.), piši o njenom životu kao o ZAVRŠENOJ cjelini.
   - LOŠE: ""Njegov rad bit će od značaja..."" (Zvuči kao da je tek rođen).
   - DOBRO: ""Njegov rad ostavio je dubok trag..."" ili ""Bio je istaknuti liječnik...""
3. Koristi PERFEKT (prošlo vrijeme), a ne Futur.
4. Iznimka: Ako pišeš o događaju osnivanja nečega što i danas postoji (npr. Muzej, Bolnica), možeš spomenuti da ""i danas djeluje"".

JSON SCHEMA:
{
""Title"": ""string (max 6 riječi, atraktivan naslov)"",
""EnhancedContent"": ""string (tečan, gramatički ispravan tekst, novinarski stil)"",
""SuggestedTypeId"": int (
Odaberi broj:
Unspecified = 0,
// --- OSOBE ---
Birth = 10,          // Rođenja (Trenk, Kučera, Thaller)
Death = 11,          // Smrti (uključuje i pogibije u ratu ako je fokus na osobi)
Biography = 12,      // Imenovanja, selidbe, vjenčanja
// --- DRUŠTVO ---
Religious = 20,      // Župe, svećenici, misije, kapele
Education = 21,      // Škole, učitelji, tečajevi
Culture = 22,        // Kazalište, glazba, knjižnice, izložbe, KUD
Sports = 23,         // Šahovski klub, planinari, ribiči
Politics = 24,       // Župani, općine, izbori, sabor, dekreti
// --- DOGAĐAJI ---
War = 30,            // Bitke, vojska, napadi, logori (Partizani, Ustaše, Turci)
Crime = 31,          // Ubojstva, pljačke (razbojnici), suđenja
Disaster = 32,       // Požari, poplave, vremenske nepogode (""Smrdi po dimu"")
// --- INFRASTRUKTURA ---
Economy = 40,        // Tvornice (Zvečevo, Orljava), obrtnici, zadruge
Infrastructure = 41, // Ceste, pruge, struja, vodovod, građevine),
""InternalNote"": ""Ovdje upiši ako si našao drugi datum ili nepravilnost (npr: 'Spominje se i smrt 1943. godine.'), ili ako događaj nema odgovarajući Type""
}",
            UserMessageTemplate = @"
ANALIZIRAJ OVAJ POVIJESNI ZAPIS.
DANAS JE GODINA: {{CURRENT_YEAR}}.
DATUM DOGAĐAJA: {{DATE}}
TEKST: ""{{CONTENT}}""

AKO JE TEKST O ROĐENJU: Započni s ""Na današnji dan rođen je..."" i istakni tko je ta osoba.
AKO JE TEKST O SMRTI: Započni s ""Na današnji dan napustio nas je...""
AKO JE RATNI DOGAĐAJ: Budi objektivan.

Ako je razlika između danas i datuma događaja velika, obavezno koristi prošlo vrijeme."
        },
        new AiPromptTemplate
        {
            Id = 2,
            Name = "Strogi Čistač v1",
            SystemInstruction = @"
Ti si arhivist i detektiv podataka.
Tvoj cilj je očistiti podatke za točno određeni datum.
Vrati isključivo JSON.

JSON OUTPUT FORMAT:
{
""Title"": ""Naslov (faktografski)"",
""EnhancedContent"": ""Tekst objave (samo činjenice za taj datum)"",
""SuggestedTypeId"": int,
""InternalNote"": ""Ovdje upiši ako si našao drugi datum ili nepravilnost (npr: 'Spominje se i smrt 1943. godine.')""
}",
            UserMessageTemplate = @"
FOKUSIRAJ SE NA OVAJ DATUM: {{DATE}}
ANALIZIRAJ OVAJ TEKST: ""{{CONTENT}}""

PRAVILA:
1. Gledaj ISKLJUČIVO događaj koji se dogodio na datum fokusa.
2. Ignoriraj događaje koji su se dogodili godinama kasnije, osim ako su nužni za kontekst.
3. Ako tekst spominje neki potpuno drugi događaj s drugim datumom, IZDVOJI ga u polje 'InternalNote'.
4. 'EnhancedContent' mora biti spreman za objavu."
        }








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
    ];

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
}