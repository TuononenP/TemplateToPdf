import {
    List,
    Datagrid,
    TextField,
    DateField,
    EditButton,
    DeleteButton,
    TextInput,
    CreateButton,
    TopToolbar,
    useRecordContext,
    useTranslate,
} from 'react-admin';
import { Button } from '@mui/material';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import { useState } from 'react';
import { TemplateTestDialog } from './TemplateTestDialog';

const ListActions = () => (
    <TopToolbar>
        <CreateButton />
    </TopToolbar>
);

const TemplateFilters = [
    <TextInput source="name" label="Search by name" alwaysOn />
];

const DeleteWithConfirmation = () => {
    const record = useRecordContext();
    return (
        <DeleteButton
            mutationMode="pessimistic"
            confirmTitle={`Delete template "${record?.name}"`}
            confirmContent="Are you sure you want to delete this template?"
        />
    );
};

const TestButton = () => {
    const record = useRecordContext();
    const [open, setOpen] = useState(false);
    const translate = useTranslate();

    if (!record) return null;

    return (
        <>
            <Button
                onClick={() => setOpen(true)}
                startIcon={<PlayArrowIcon />}
                size="small"
            >
                {translate('templates.test.action')}
            </Button>
            <TemplateTestDialog
                open={open}
                onClose={() => setOpen(false)}
                template={record}
            />
        </>
    );
};

export const TemplateList = () => (
    <List
        actions={<ListActions />}
        filters={TemplateFilters}
        sort={{ field: 'updatedAt', order: 'DESC' }}
    >
        <Datagrid>
            <TextField source="id" />
            <TextField source="name" label="Name" />
            <DateField source="createdAt" showTime label="Created" />
            <DateField source="updatedAt" showTime label="Updated" />
            <TestButton />
            <EditButton />
            <DeleteWithConfirmation />
        </Datagrid>
    </List>
); 