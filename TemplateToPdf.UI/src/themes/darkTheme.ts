import { defaultTheme } from 'react-admin';

export const darkTheme = {
    ...defaultTheme,
    palette: {
        mode: 'dark',
        primary: {
            main: '#90caf9',
        },
        background: {
            default: '#121212',
            paper: '#1e1e1e',
        },
    },
    components: {
        ...defaultTheme.components,
        MuiAppBar: {
            styleOverrides: {
                colorSecondary: {
                    backgroundColor: '#1e1e1e',
                    color: '#ffffff',
                },
            },
        },
        MuiDrawer: {
            styleOverrides: {
                paper: {
                    backgroundColor: '#121212',
                    color: '#ffffff',
                },
            },
        },
    },
}; 