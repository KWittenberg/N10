export async function copyCardToClipboard(elementId) {
    const element = document.getElementById(elementId);
    if (!element) {
        console.error("Ne mogu naći element: " + elementId);
        return;
    }

    try {
        if (typeof html2canvas === 'undefined') {
            alert("Greška: html2canvas biblioteka nije učitana!");
            return;
        }

        const canvas = await html2canvas(element, {
            scale: 2, // HD kvaliteta
            backgroundColor: null, // Pustimo da onclone rješava pozadinu
            useCORS: true,
            logging: false,

            // --- OVDJE JE MAGIJA ---
            onclone: (clonedDoc) => {
                const clonedElement = clonedDoc.getElementById(elementId);
                if (clonedElement) {
                    // 1. Resetiramo flex/grid da se ne rasteže
                    clonedElement.style.display = "block";
                    clonedElement.style.flex = "none";

                    // 2. ZAKUCAVAMO ŠIRINU (Idealno za FB/Insta story format)
                    clonedElement.style.width = "600px";
                    clonedElement.style.height = "auto";

                    // 3. Dodajemo padding i crnu pozadinu (okvir)
                    clonedElement.style.padding = "40px";
                    clonedElement.style.backgroundColor = "#09090b";
                    clonedElement.style.borderRadius = "0"; // Ipak bez roundanja vanjskog ruba za sliku

                    // 4. Centriramo sadržaj unutar tog crnog okvira
                    clonedElement.style.margin = "0";
                }
            }
        });

        canvas.toBlob(async (blob) => {
            try {
                const item = new ClipboardItem({ "image/png": blob });
                await navigator.clipboard.write([item]);
                alert("Slika je kopirana! 📸 Zalijepi je na Face.");
            } catch (err) {
                console.error("Clipboard error:", err);
                alert("Browser ne da kopiranje. Probaj drugi browser.");
            }
        });

    } catch (error) {
        console.error("Greška kod slikanja:", error);
    }
}