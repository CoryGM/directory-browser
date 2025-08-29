function getQueryParams() {
    const params = {};
    const queryString = window.location.search.substring(1);

    queryString.split("&").forEach(pair => {
        if (!pair) return;
        const [key, value] = pair.split("=");
        params[decodeURIComponent(key)] = decodeURIComponent((value || "").replace(/\+/g, " "));
    });

    return params;
}

function setupBrowseButtonListener() {
    const browseButton = document.getElementById('browse-button');

    if (!browseButton) return;

    browseButton.addEventListener('click', () => {
        const currentPath = document.getElementById('current-path-input')?.value ?? '';
        pushStateAndGetData(currentPath);
    });
}

function setupDirectoryLinkListeners() {
    document.querySelectorAll('.browse-directory-link').forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();

            const currentPath = document.getElementById('current-path-input')?.value ?? '';
            const currentSubPath = decodeURIComponent(this.getAttribute('data-dir'));
            let newCurrentPath = currentSubPath;

            if (currentPath && currentPath !== '\\' && currentPath !== '/')
                newCurrentPath = currentPath + currentSubPath;

            pushStateAndGetData(newCurrentPath);
        });
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

function pushStateAndGetData(currentPath) {
    const searchTerm = document.getElementById('search-term-input')?.value ?? '';
    const includeSubdirs = document.getElementById('include-subdirectories-input')?.checked ? 'true' : 'false';

    // Build query string
    const params = new URLSearchParams();

    if (currentPath)
        params.append('currentPath', currentPath);

    if (searchTerm)
        params.append('searchTerm', searchTerm);

    params.append('includeSubdirectories', includeSubdirs);
    const paramsString = params.toString();

    window.history.pushState(null, null, `/browse?${paramsString}`);

    getData();
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
    let html = '';

    if (data.currentPath !== '\\' && data.currentPath !== '/') {
        html +=
            `<div class="browse-file-tab-current-path">
                <i class="fa-solid fa-folder-open"></i> ${data.currentPath}
            </div>`;
    }

    if (!data || !Array.isArray(data.matchedFileDirectories) || !data.matchedFileDirectories.length)
        return html += '<div> No files found</div>';

    html +=
        `<table class="browse-file-tab-table">
            <thead>
                <th class="browse-file-tab-file-name-header">File Name</th>
                <th class="browse-file-tab-file-size-header">Size</th>
            <thead>
        <tbody>`;

    data.matchedFileDirectories.forEach((matchedFileDirectory, index) => {
        html +=
            `<tr>
                <td class="browse-file-tab-directory-name-cell">
                    <i class="fa-solid fa-folder"></i>`;

        if (index === 0) {
            html += `${matchedFileDirectory.name}`;
        } else {
            html += `<a href="#" class="browse-directory-link" data-dir="${encodeURIComponent(matchedFileDirectory.name)}">${matchedFileDirectory.name}</a>`;
        }

        html +=
               `</td >
                <td class="browse-file-tab-directory-size-cell">
                    ${ matchedFileDirectory.fileCount } files, ${ matchedFileDirectory.size }
                </td>
            </tr >`;

        matchedFileDirectory.files.forEach((matchedFile, fileIndex) => {
            html +=
                `<tr>
                    <td class="browse-file-tab-file-name-cell">
                        <i class="fa-regular fa-file"></i> ${matchedFile.name}
                    </td>
                    <td class="browse-file-tab-file-size-cell">
                        ${matchedFile.size}
                    </td>
                 </tr>`;
        });
    }); 

    html += '</tbody></table>';

    return html;
}

async function getData() {
    const queryParams = getQueryParams();
    let url = '/api/directories/browse';

    const params = new URLSearchParams(queryParams);
    const queryString = params.toString();

    if (queryString) {
        url += `?${queryString}`;
    }

    const response = await fetch(url);
    const data = await response.json();

    renderTabContent(data);

    setDisplayValuesAfterGetData(data);
    setupDirectoryLinkListeners();
}

function setDisplayValuesAfterGetData(data) {
    const currentPathInput = document.getElementById('current-path-input');
    const currentPathText = document.getElementById('current-path-text');

    if (data.currentPath && currentPathText) {
        currentPathText.innerHTML = decodeURIComponent(data.currentPath);
    }

    if (currentPathInput) {
        if (data.currentPath) {
            currentPathInput.value = data.currentPath;
        } else {
            currentPathInput.value = '';
        }
    }
}

function setFormValuesAfterRender() {
    const queryParams = getQueryParams();
    const currentPathInput = document.getElementById('current-path-input');
    const searchTermInput = document.getElementById('search-term-input');
    const includeSubdirsInput = document.getElementById('include-subdirectories-input');

    if (currentPathInput) {
        if (queryParams.currentPath) {
            currentPathInput.value = queryParams.currentPath;
        } else {
            currentPathInput.value = '';
        }
    }

    if (searchTermInput) {
        if (queryParams.searchTerm) {
            searchTermInput.value = queryParams.searchTerm;
        } else {
            searchTermInput.value = '';
        }
    }

    if (includeSubdirsInput) {
        if (queryParams.includeSubdirectories) {
            includeSubdirsInput.checked = queryParams.includeSubdirectories.toLowerCase() === 'true';
        } else {
            includeSubdirsInput.checked = false;
        }
    }
}

export async function renderBrowsePage() {
    setupBrowseButtonListener();
    setupTabListeners();
    setFormValuesAfterRender();

    await getData();
}