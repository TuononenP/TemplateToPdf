import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent, CardActions, Button, FormControl, InputLabel, Select, MenuItem, Box } from '@mui/material';
import { Title, useTranslate } from 'react-admin';

type Language = 'en' | 'fi' | 'sv';
type Theme = 'light' | 'dark';

interface Settings {
    language: Language;
    theme: Theme;
}

const SettingsPage = () => {
    const navigate = useNavigate();
    const translate = useTranslate();
    const [settings, setSettings] = useState<Settings>(() => ({
        language: (localStorage.getItem('language') as Language) || 'en',
        theme: (localStorage.getItem('theme') as Theme) || 'light'
    }));

    const handleThemeChange = (theme: Theme) => {
        setSettings(prev => ({ ...prev, theme }));
        localStorage.setItem('theme', theme);
        // Force reload to apply the new theme
        window.location.reload();
    };

    const handleLanguageChange = (language: Language) => {
        setSettings(prev => ({ ...prev, language }));
        localStorage.setItem('language', language);
        // Force reload to apply the new language
        window.location.reload();
    };

    const handleSave = () => {
        localStorage.setItem('language', settings.language);
        navigate('/');
        window.location.reload();
    };

    return (
        <Card>
            <Title title={translate('settings.title')} />
            <CardContent>
                <Box display="flex" flexDirection="column" gap={3}>
                    <FormControl fullWidth>
                        <InputLabel>{translate('settings.language')}</InputLabel>
                        <Select
                            value={settings.language}
                            label={translate('settings.language')}
                            onChange={(e) => handleLanguageChange(e.target.value as Language)}
                        >
                            <MenuItem value="en">English</MenuItem>
                            <MenuItem value="fi">Suomi</MenuItem>
                            <MenuItem value="sv">Svenska</MenuItem>
                        </Select>
                    </FormControl>

                    <FormControl fullWidth>
                        <InputLabel>{translate('settings.theme')}</InputLabel>
                        <Select
                            value={settings.theme}
                            label={translate('settings.theme')}
                            onChange={(e) => handleThemeChange(e.target.value as Theme)}
                        >
                            <MenuItem value="light">{translate('settings.themes.light')}</MenuItem>
                            <MenuItem value="dark">{translate('settings.themes.dark')}</MenuItem>
                        </Select>
                    </FormControl>
                </Box>
            </CardContent>
            <CardActions sx={{ padding: 2, justifyContent: 'space-between' }}>
                <Button onClick={() => navigate('/')} color="inherit">
                    {translate('ra.action.back')}
                </Button>
                <Box>
                    <Button onClick={() => navigate('/')} color="inherit" sx={{ mr: 1 }}>
                        {translate('ra.action.cancel')}
                    </Button>
                    <Button onClick={handleSave} variant="contained" color="primary">
                        {translate('ra.action.save')}
                    </Button>
                </Box>
            </CardActions>
        </Card>
    );
};

export default SettingsPage; 