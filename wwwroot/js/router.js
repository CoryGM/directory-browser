export async function loadPage(route) {
    const res = await fetch(route);
    return await res.text();
}
