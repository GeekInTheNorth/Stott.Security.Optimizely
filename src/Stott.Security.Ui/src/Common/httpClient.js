const buildQueryString = (params) => {
    if (!params) return '';
    const search = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
            search.append(key, value instanceof Date ? value.toISOString() : value);
        }
    });
    const query = search.toString();
    return query ? `?${query}` : '';
};

const appendQueryString = (url, params) => {
    const query = buildQueryString(params);
    if (!query) return url;
    return url + (url.includes('?') ? `&${query.slice(1)}` : query);
};

const parseBody = async (response) => {
    const contentType = response.headers.get('Content-Type') || '';
    const text = await response.text();
    if (!text) return null;
    if (contentType.toLowerCase().includes('application/json')) {
        try {
            return JSON.parse(text);
        } catch {
            return text;
        }
    }
    return text;
};

const request = async (url, { method, body, params } = {}) => {
    const headers = { Accept: 'application/json' };
    let payload;
    if (body instanceof URLSearchParams) {
        payload = body;
    } else if (body !== undefined && body !== null) {
        headers['Content-Type'] = 'application/json';
        payload = JSON.stringify(body);
    }

    const response = await fetch(appendQueryString(url, params), {
        method,
        headers,
        body: payload,
        credentials: 'same-origin'
    });

    const data = await parseBody(response);

    if (!response.ok) {
        const error = new Error(`HTTP ${response.status}`);
        error.response = { status: response.status, data };
        throw error;
    }

    return { data };
};

export const httpGet = (url, params) => request(url, { method: 'GET', params });
export const httpPost = (url, body, params) => request(url, { method: 'POST', body, params });
export const httpDelete = (url, params) => request(url, { method: 'DELETE', params });
