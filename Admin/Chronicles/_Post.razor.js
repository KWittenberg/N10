export async function getCardImageStream(elementId) {
    const element = document.getElementById(elementId);
    if (!element) return null;

    try {
        if (typeof html2canvas === 'undefined') return null;

        const canvas = await html2canvas(element, {
            scale: 1,
            backgroundColor: '#09090b',
            useCORS: true,
            logging: false,
            onclone: (clonedDoc) => {
                const clonedElement = clonedDoc.getElementById(elementId);
                if (clonedElement) {
                    clonedElement.style.width = "600px";
                    clonedElement.style.height = "auto";
                    clonedElement.style.padding = "40px";
                    clonedElement.style.margin = "0";
                    clonedElement.style.backgroundColor = "#09090b";
                }
            }
        });

        // Vraća string tipa "data:image/png;base64,iVBORw0KGgo..."
        // return canvas.toDataURL("image/jpeg", 0.9);

        const blob = await new Promise(resolve => canvas.toBlob(resolve, "image/jpeg", 0.85));

        const arrayBuffer = await blob.arrayBuffer();
        return new Uint8Array(arrayBuffer);

    } catch (error) {
        console.error("Greška:", error);
        return null;
    }
}