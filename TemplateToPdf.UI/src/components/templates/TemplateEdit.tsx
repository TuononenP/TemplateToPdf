import {
    Edit,
    SimpleForm,
    TextInput,
    required,
    TopToolbar,
    Button,
    SaveButton,
    DeleteButton,
    Toolbar
} from 'react-admin';
import { Link } from 'react-router-dom';
import ArrowBack from '@mui/icons-material/ArrowBack';
import { Box } from '@mui/material';

const EditActions = () => (
    <TopToolbar>
        <Button
            component={Link}
            to="/templates"
            label="Back"
            startIcon={<ArrowBack />}
        />
    </TopToolbar>
);

const EditToolbar = () => (
    <Toolbar>
        <Box flex={1} display="flex" gap={1}>
            <SaveButton />
        </Box>
        <DeleteButton
            mutationMode="pessimistic"
            confirmTitle="Delete template"
            confirmContent="Are you sure you want to delete this template?"
        />
    </Toolbar>
);

export const TemplateEdit = () => (
    <Edit 
        actions={<EditActions />}
        mutationMode="pessimistic"
    >
        <SimpleForm toolbar={<EditToolbar />}>
            <TextInput source="name" validate={[required()]} fullWidth />
            <TextInput
                source="content"
                validate={[required()]}
                multiline
                rows={20}
                fullWidth
            />
        </SimpleForm>
    </Edit>
); 