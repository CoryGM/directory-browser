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

function setupTabListeners() {
    const tabButtons = document.querySelectorAll('.tab-btn');
    tabButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            tabButtons.forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            const tab = btn.getAttribute('data-tab');
            document.getElementById('tab-directories').style.display = tab === 'directories' ? 'block' : 'none';
            document.getElementById('tab-files').style.display = tab === 'files' ? 'block' : 'none';
            document.getElementById('tab-raw-data').style.display = tab === 'raw-data' ? 'block' : 'none';
        });
    });
}

function renderTabContent(data) {
    // Directories
    const dirDiv = document.getElementById('tab-directories');
    if (Array.isArray(data.matchedDirectories)) {
        dirDiv.innerHTML = data.matchedDirectories.length
            ? `<ul>${data.matchedDirectories.map(d => `<li>${d}</li>`).join('')}</ul>`
            : '<div>No directories found.</div>';
    } else {
        dirDiv.innerHTML = '<div>No directories found.</div>';
    }

    // Files
    const fileDiv = document.getElementById('tab-files');
    fileDiv.innerHTML = renderFileTabHtml(data);

    // Raw Data
    const rawDataDiv = document.getElementById('tab-raw-data');
    rawDataDiv.innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
}

function renderFileTabHtml(data) {
    if (!data || !Array.isArray(data.matchedFiles) || !data.matchedFiles.length)
        return '<div> No files found</div>';

    let lastPath = '';

    let html = '<table class="browse-file-tab-table"><thead><th class="browse-file-tab-file-name-header">File Name</th><th class="browse-file-tab-file-size-header">Size</th><thead><tbody>';

    data.matchedFiles.forEach((filePathName, index) => {
        const [path, fileName] = splitPathAndFileName(filePathName);

        if (lastPath !== path) {
            html += `<tr><td colspan="2" class="browse-file-tab-directory-cell">${path}</td></tr>`;
        }

        html += `<tr><td class="browse-file-tab-file-name-cell">${fileName}</td><td class="browse-file-tab-file-size-cell">TBD</td></tr>`;

        lastPath = path;
    }); 

    html += '</tbody></table>';

    return html;
}

function splitPathAndFileName(filePathName) {
    if (!filePathName)
        return ['', ''];

    let rootPath = "\\";

    let lastSlash = filePathName.lastIndexOf('\\');
    let path = '';

    if (lastSlash < 0) {
        lastSlash = filePathName.lastIndexOf('/');

        if (lastSlash >= 0)
            rootPath = "/";
    }

    if (lastSlash < 0)
        return [rootPath, filePathName];

    if (lastSlash !== 0)
        path = filePathName.substring(0, lastSlash);
    else
        path = rootPath;

    const fileName = filePathName.substring(lastSlash + 1);

    return [path, fileName];
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

    renderTabContent(data);
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
    setupTabListeners();
    setFormValuesAfterRender();

    await getData();
}