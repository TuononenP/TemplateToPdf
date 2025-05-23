import { Admin, Resource, CustomRoutes } from 'react-admin';
import { i18nProvider } from './i18n/i18nProvider';
import { TemplateList, TemplateEdit, TemplateCreate } from './components/templates';
import { AssetList, AssetEdit, AssetCreate } from './components/assets';
import { SettingsPage } from './components/settings';
import { dataProvider } from './dataProvider';
import { Layout } from './layout';
import Dashboard from './components/Dashboard';
import DescriptionIcon from '@mui/icons-material/Description';
import InsertDriveFileIcon from '@mui/icons-material/InsertDriveFile';
import { Route } from 'react-router-dom';
import { darkTheme, lightTheme } from './themes';

const App = () => {
    const savedTheme = localStorage.getItem('theme') || 'light';
    
    return (
        <Admin 
            dataProvider={dataProvider}
            i18nProvider={i18nProvider}
            layout={Layout}
            dashboard={Dashboard}
            title="Dashboard"
            theme={savedTheme === 'dark' ? darkTheme : lightTheme}
            requireAuth={false}
        >
            <CustomRoutes>
                <Route path="/settings" element={<SettingsPage />} />
            </CustomRoutes>
            <Resource
                name="templates"
                options={{ label: 'Templates' }}
                list={TemplateList}
                edit={TemplateEdit}
                create={TemplateCreate}
                icon={DescriptionIcon}
            />
            <Resource
                name="assets"
                options={{ label: 'Assets' }}
                list={AssetList}
                edit={AssetEdit}
                create={AssetCreate}
                icon={InsertDriveFileIcon}
            />
        </Admin>
    );
};

export default App; 