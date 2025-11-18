// Infinite scroll implementacija
export function registerInfiniteScroll(dotNetHelper) {
    let isLoading = false;

    window.addEventListener('scroll', async function () {
        if (isLoading) return;

        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        const windowHeight = window.innerHeight;
        const documentHeight = document.documentElement.scrollHeight;

        // Učitaj kada je 200px do dna
        if (scrollTop + windowHeight >= documentHeight - 200) {
            isLoading = true;
            try {
                await dotNetHelper.invokeMethodAsync('LoadMoreJS');
            } catch (error) {
                console.error('Error loading more data:', error);
            }
            isLoading = false;
        }
    });
}

// Manual trigger za load more
export function scrollToLoadMore() {
    const loadMoreButton = document.querySelector('[onclick*="LoadMoreData"]');
    if (loadMoreButton) {
        loadMoreButton.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
}