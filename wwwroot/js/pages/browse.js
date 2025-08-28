function getQueryParams() {
    const params = {};
    const queryString = window.location.search.substring(1);
    queryString.split("&").forEach(pair => {
        if (!pair) return;
        const [key, value] = pair.split("=");
        params[decodeURIComponent(key)] = decodeURIComponent(value || "");
    });
    return params;
}

function setupBrowseButtonListener() {
    const browseButton = document.getElementById('browse-button');

    if (!browseButton) return;

    browseButton.addEventListener('click', () => {
        const searchTerm = document.getElementById('search-term-input')?.value ?? '';
        const includeSubdirs = document.getElementById('include-subdirectories-input')?.checked ? 'true' : 'false';

        // Build query string
        const params = new URLSearchParams();
        if (searchTerm) params.append('searchTerm', searchTerm);
        params.append('includeSubdirectories', includeSubdirs);

        // Update window path state
        const newUrl = `/browse?${params.toString()}`;
        window.history.pushState(null, null, newUrl);

        getData();
    });
}

async function getData() {
    const queryParams = getQueryParams();
    let url = '/api/directories/browse';

    const queryString = Object.keys(queryParams)
        .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(queryParams[key])}`)
        .join("&");

    if (queryString) {
        url += `?${queryString}`;
    }

    const response = await fetch(url);
    const data = await response.json();

    // Only update the #browse-result div
    const resultDiv = document.getElementById('browse-result');
    resultDiv.innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
}

function setFormValuesAfterRender() {
    const queryParams = getQueryParams();
    const searchTermInput = document.getElementById('search-term-input');
    const includeSubdirsInput = document.getElementById('include-subdirectories-input');

    if (queryParams.searchTerm && searchTermInput) {
        searchTermInput.value = queryParams.searchTerm;
    }

    if (queryParams.includeSubdirectories && includeSubdirsInput) {
        includeSubdirsInput.checked = queryParams.includeSubdirectories.toLowerCase() === 'true';
    }
}

export async function renderBrowsePage() {
    setupBrowseButtonListener();
    setFormValuesAfterRender();

    await getData();
}