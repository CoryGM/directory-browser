import { loadPage } from './router.js';
import { renderBrowsePage } from './pages/browse.js';

const routes = {
    '/': { route: '/pages/browse.html', method: renderBrowsePage },
    '/browse': { route: '/pages/browse.html', method: renderBrowsePage },
    '/contact': { route: '/pages/contact.html' }
};
function navigateTo(url) {
    history.pushState(null, null, url);
    router();
}

window.addEventListener('popstate', router);

function normalizePath(path) {
    const indexKey = 'index.html';
    var indexHtmlPos = window.location.pathname.toLowerCase().indexOf(indexKey);

    // Need to remove the /index.html part for correct routing but a) we don't know the casing
    // of the "/index.html" value and b) we need to preserve the casing of the rest of the path
    if (indexHtmlPos !== -1) {
        path = path.substring(0, indexHtmlPos) + path.substring(indexHtmlPos + indexKey.length);
    };

    return path
}

async function router() {
    const rawPath = window.location.pathname;
    const path = normalizePath(rawPath);

    if (rawPath !== path)
        history.replaceState(null, null, path);

    const html = await loadPage(routes[path].route || '/pages/404.html');
    const appDiv = document.getElementById('app');
    appDiv.classList.add('app-content');
    appDiv.innerHTML = html;

    if (routes[path].method) {
        routes[path].method();
    };
}

window.addEventListener('popstate', router);

document.addEventListener('DOMContentLoaded', () => {
    document.body.addEventListener('click', e => {
        if (e.target.matches('[data-link]')) {
            e.preventDefault();
            navigateTo(e.target.href);
        }
    });
    router();
});
