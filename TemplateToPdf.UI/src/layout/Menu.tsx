import { Menu as ReactAdminMenu, MenuItemLink } from 'react-admin';
import DashboardIcon from '@mui/icons-material/Dashboard';
import SettingsIcon from '@mui/icons-material/Settings';
import DescriptionIcon from '@mui/icons-material/Description';
import InsertDriveFileIcon from '@mui/icons-material/InsertDriveFile';

const menuItemStyles = {
    '&.RaMenuItemLink-active': {
        backgroundColor: 'action.selected',
    },
    '&:hover': {
        backgroundColor: 'action.hover',
    },
};

export const Menu = () => (
    <ReactAdminMenu>
        <MenuItemLink
            to="/"
            primaryText="Dashboard"
            leftIcon={<DashboardIcon />}
            sx={menuItemStyles}
        />
        <MenuItemLink
            to="/templates"
            primaryText="Templates"
            leftIcon={<DescriptionIcon />}
            sx={menuItemStyles}
        />
        <MenuItemLink
            to="/assets"
            primaryText="Assets"
            leftIcon={<InsertDriveFileIcon />}
            sx={menuItemStyles}
        />
        <MenuItemLink
            to="/settings"
            primaryText="Settings"
            leftIcon={<SettingsIcon />}
            sx={menuItemStyles}
        />
    </ReactAdminMenu>
); 