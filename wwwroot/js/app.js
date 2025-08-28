import { loadPage } from './router.js';

const routes = {
    '/': '/pages/browse.html',
    '/contact': '/pages/contact.html'
};
function navigateTo(url) {
    history.pushState(null, null, url);
    router();
}

window.addEventListener('popstate', router);

function normalizePath(path) {
    return path === '/index.html' ? '/' : path;
}

async function router() {
    const rawPath = window.location.pathname;
    const path = normalizePath(rawPath);
    const html = await loadPage(routes[path] || '/pages/404.html');
    const appDiv = document.getElementById('app');
    appDiv.classList.add('app-content');
    appDiv.innerHTML = html;
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
