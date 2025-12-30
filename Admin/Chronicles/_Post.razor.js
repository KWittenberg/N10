export async function getCardImageBase64(elementId) {
    const element = document.getElementById(elementId);
    if (!element) return null;

    try {
        if (typeof html2canvas === 'undefined') return null;

        const canvas = await html2canvas(element, {
            scale: 2,
            backgroundColor: null, // Pustimo onclone da oboji
            useCORS: true,
            logging: false,
            onclone: (clonedDoc) => {
                const clonedElement = clonedDoc.getElementById(elementId);
                if (clonedElement) {
                    // Tvoje postavke za Facebook (600px širina, crna pozadina)
                    clonedElement.style.display = "block";
                    clonedElement.style.flex = "none";
                    clonedElement.style.width = "600px";
                    clonedElement.style.height = "auto";
                    clonedElement.style.padding = "40px";
                    clonedElement.style.backgroundColor = "#09090b";
                    clonedElement.style.margin = "0";
                }
            }
        });

        // Vraća string tipa "data:image/png;base64,iVBORw0KGgo..."
        return canvas.toDataURL("image/png");

    } catch (error) {
        console.error("Greška:", error);
        return null;
    }
}