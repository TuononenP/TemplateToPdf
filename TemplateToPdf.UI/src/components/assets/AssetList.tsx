import {
    List,
    Datagrid,
    TextField,
    DateField,
    EditButton,
    DeleteButton,
    TextInput,
    SelectInput,
    TopToolbar,
    CreateButton,
    FilterButton,
    useTranslate,
    useRecordContext,
    Button,
    FunctionField,
} from 'react-admin';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button as MuiButton } from '@mui/material';
import { useState } from 'react';
import PreviewIcon from '@mui/icons-material/Preview';
import AceEditor from 'react-ace';
import { AssetType } from 'types';

// Import ace editor themes and modes
import 'ace-builds/webpack-resolver';
import 'ace-builds/src-min-noconflict/ext-language_tools';
import 'ace-builds/src-min-noconflict/mode-css';
import 'ace-builds/src-min-noconflict/mode-handlebars';
import 'ace-builds/src-min-noconflict/theme-twilight';
import 'ace-builds/src-min-noconflict/theme-xcode';

const ListActions = () => (
    <TopToolbar>
        <FilterButton />
        <CreateButton />
    </TopToolbar>
);

const AssetFilters = [
    <TextInput source="name" alwaysOn />,
    <TextInput source="referenceName" />,
    <SelectInput
        source="type"
        choices={[
            { id: AssetType.Image, name: 'assets.types.image' },
            { id: AssetType.Css, name: 'assets.types.css' },
            { id: AssetType.Font, name: 'assets.types.font' },
            { id: AssetType.PartialTemplate, name: 'assets.types.partialTemplate' },
        ]}
        translateChoice
    />,
];

interface Asset {
    id: number;
    name: string;
    referenceName: string;
    type: AssetType;
    mimeType?: string;
    content?: string;
    binaryContent?: string;
    description?: string;
    createdAt: string;
    updatedAt: string;
}

interface PreviewDialogProps {
    open: boolean;
    onClose: () => void;
    record: Asset | null;
}

const PreviewDialog = ({ open, onClose, record }: PreviewDialogProps) => {
    const translate = useTranslate();
    const theme = localStorage.getItem('theme') || 'light';

    const renderPreview = () => {
        if (!record) return null;

        switch (record.type) {
            case AssetType.Image:
                return (
                    <img
                        src={`data:${record.mimeType};base64,${record.binaryContent}`}
                        alt={record.name}
                        style={{ maxWidth: '100%', maxHeight: '500px' }}
                    />
                );
            case AssetType.Css:
                return (
                    <AceEditor
                        mode="css"
                        theme={theme === 'dark' ? 'twilight' : 'xcode'}
                        value={record.content || ''}
                        readOnly
                        width="100%"
                        height="400px"
                        setOptions={{
                            showLineNumbers: true,
                            tabSize: 2,
                            useWorker: false,
                        }}
                    />
                );
            case AssetType.PartialTemplate:
                return (
                    <AceEditor
                        mode="handlebars"
                        theme={theme === 'dark' ? 'twilight' : 'xcode'}
                        value={record.content || ''}
                        readOnly
                        width="100%"
                        height="400px"
                        setOptions={{
                            showLineNumbers: true,
                            tabSize: 2,
                            useWorker: false,
                        }}
                    />
                );
            case AssetType.Font:
                return (
                    <div>
                        <style>
                            {`
                            @font-face {
                                font-family: '${record.name}';
                                src: url(data:font/woff2;base64,${record.binaryContent}) format('woff2');
                            }
                            .preview-text {
                                font-family: '${record.name}', sans-serif;
                            }
                            `}
                        </style>
                        <div className="preview-text" style={{ fontSize: '24px', marginBottom: '20px' }}>
                            {translate('assets.preview.fontPreviewText')}
                        </div>
                    </div>
                );
            default:
                return <div>{translate('assets.preview.unsupportedType')}</div>;
        }
    };

    return (
        <Dialog
            open={open}
            onClose={onClose}
            maxWidth="md"
            fullWidth
        >
            <DialogTitle>
                {translate('assets.preview.title', { name: record?.name })}
            </DialogTitle>
            <DialogContent>
                {renderPreview()}
            </DialogContent>
            <DialogActions>
                <MuiButton onClick={onClose}>
                    {translate('ra.action.close')}
                </MuiButton>
            </DialogActions>
        </Dialog>
    );
};

const PreviewButton = () => {
    const [open, setOpen] = useState(false);
    const record = useRecordContext<Asset>();
    const translate = useTranslate();

    if (!record) return null;

    return (
        <>
            <Button
                onClick={() => setOpen(true)}
                label={translate('assets.preview.action')}
                startIcon={<PreviewIcon />}
            />
            <PreviewDialog
                open={open}
                onClose={() => setOpen(false)}
                record={record}
            />
        </>
    );
};

const AssetTypeField = (props: any) => {
    const record = useRecordContext();
    const translate = useTranslate();
    if (!record) return null;

    const typeMap = {
        [AssetType.Image]: translate('assets.types.image'),
        [AssetType.Css]: translate('assets.types.css'),
        [AssetType.Font]: translate('assets.types.font'),
        [AssetType.PartialTemplate]: translate('assets.types.partialTemplate')
    };

    return <FunctionField {...props} render={() => typeMap[record.type as AssetType]} />;
};

export const AssetList = () => (
    <List
        actions={<ListActions />}
        filters={AssetFilters}
        sort={{ field: 'updatedAt', order: 'DESC' }}
    >
        <Datagrid
            sx={{
                '& .RaDatagrid-headerCell': {
                    backgroundColor: theme => theme.palette.mode === 'light' ? '#f5f5f5' : '#333333',
                    fontWeight: 'bold'
                },
            }}
        >
            <TextField source="id" />
            <TextField source="name" />
            <TextField source="referenceName" />
            <AssetTypeField label="assets.fields.type" />
            <DateField source="createdAt" showTime />
            <DateField source="updatedAt" showTime />
            <PreviewButton />
            <EditButton />
            <DeleteButton
                mutationMode="pessimistic"
            />
        </Datagrid>
    </List>
); 