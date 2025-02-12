import { fetchUtils } from 'react-admin';
import qs from 'query-string';

const apiUrl = process.env.REACT_APP_API_URL || 'http://localhost:6050/api';
const httpClient = fetchUtils.fetchJson;

export const dataProvider = {
    getList: async (resource: string, params: any) => {
        const { page, perPage } = params.pagination;
        const { field, order } = params.sort;
        
        // Build query parameters
        const query: any = {
            page: page,
            perPage: perPage,
            sort: field,
            order: order,
        };

        // Add name filter if present
        if (params.filter.name) {
            query.name = params.filter.name;
        }

        const url = `${apiUrl}/${resource}?${qs.stringify(query)}`;
        const { json } = await httpClient(url);
        return {
            data: json,
            total: json.length,
        };
    },

    getOne: async (resource: string, params: any) => {
        const { json } = await httpClient(`${apiUrl}/${resource}/${params.id}`);
        return {
            data: json,
        };
    },

    getMany: async (resource: string, params: any) => {
        const query = {
            filter: JSON.stringify({ id: params.ids }),
        };
        const url = `${apiUrl}/${resource}?${qs.stringify(query)}`;
        const { json } = await httpClient(url);
        return {
            data: json,
        };
    },

    getManyReference: async (resource: string, params: any) => {
        const { page, perPage } = params.pagination;
        const { field, order } = params.sort;
        const query = {
            sort: field,
            order: order,
            page: page,
            perPage: perPage,
            filter: JSON.stringify({
                ...params.filter,
                [params.target]: params.id,
            }),
        };
        const url = `${apiUrl}/${resource}?${qs.stringify(query)}`;

        const { json } = await httpClient(url);
        return {
            data: json,
            total: json.length,
        };
    },

    create: async (resource: string, params: any) => {
        const { json } = await httpClient(`${apiUrl}/${resource}`, {
            method: 'POST',
            body: JSON.stringify(params.data),
        });
        return {
            data: { ...params.data, id: json.id },
        };
    },

    update: async (resource: string, params: any) => {
        const { json } = await httpClient(`${apiUrl}/${resource}/${params.id}`, {
            method: 'PUT',
            body: JSON.stringify(params.data),
        });
        return {
            data: json,
        };
    },

    updateMany: async (resource: string, params: any) => {
        const query = {
            filter: JSON.stringify({ id: params.ids}),
        };
        const { json } = await httpClient(`${apiUrl}/${resource}?${qs.stringify(query)}`, {
            method: 'PUT',
            body: JSON.stringify(params.data),
        });
        return {
            data: json,
        };
    },

    delete: async (resource: string, params: any) => {
        await httpClient(`${apiUrl}/${resource}/${params.id}`, {
            method: 'DELETE',
        });
        return {
            data: params.previousData,
        };
    },

    deleteMany: async (resource: string, params: any) => {
        const query = {
            filter: JSON.stringify({ id: params.ids}),
        };
        await httpClient(`${apiUrl}/${resource}?${qs.stringify(query)}`, {
            method: 'DELETE',
        });
        return {
            data: [],
        };
    },
}; 