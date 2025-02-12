import { defaultTheme } from 'react-admin';

export const lightTheme = {
    ...defaultTheme,
    palette: {
        mode: 'light',
        primary: {
            main: '#1976d2',
        },
        background: {
            default: '#fafafb',
        },
    },
    components: {
        ...defaultTheme.components,
        MuiAppBar: {
            styleOverrides: {
                colorSecondary: {
                    backgroundColor: '#0380fc',
                    color: '#ffffff',
                },
            },
        },
    },
}; 