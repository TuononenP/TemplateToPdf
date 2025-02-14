import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Box,
    Typography,
} from '@mui/material';
import { useTranslate } from 'react-admin';
import AceEditor from 'react-ace';

interface AssetPreviewDialogProps {
    open: boolean;
    onClose: () => void;
    asset: {
        id: number;
        name: string;
        type: string;
        content?: string;
        binaryContent?: string;
        mimeType?: string;
    };
}

export const AssetPreviewDialog = ({ open, onClose, asset }: AssetPreviewDialogProps) => {
    const translate = useTranslate();
    const theme = localStorage.getItem('theme') || 'light';

    const renderPreview = () => {
        switch (asset.type) {
            case 'Image':
                return (
                    <Box sx={{ textAlign: 'center' }}>
                        <img
                            src={`data:${asset.mimeType};base64,${asset.binaryContent}`}
                            alt={asset.name}
                            style={{ maxWidth: '100%', maxHeight: '500px' }}
                        />
                    </Box>
                );
            case 'Css':
                return (
                    <AceEditor
                        mode="css"
                        theme={theme === 'dark' ? 'twilight' : 'xcode'}
                        value={asset.content || ''}
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
            case 'PartialTemplate':
                return (
                    <AceEditor
                        mode="handlebars"
                        theme={theme === 'dark' ? 'twilight' : 'xcode'}
                        value={asset.content || ''}
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
            case 'Font':
                return (
                    <Box>
                        <style>
                            {`
                            @font-face {
                                font-family: '${asset.name}';
                                src: url(data:font/woff2;base64,${asset.binaryContent}) format('woff2');
                            }
                            .preview-text {
                                font-family: '${asset.name}', sans-serif;
                            }
                            `}
                        </style>
                        <Typography variant="h4" className="preview-text" gutterBottom>
                            {translate('assets.preview.fontPreviewTitle')}
                        </Typography>
                        <Typography className="preview-text">
                            {translate('assets.preview.fontPreviewText')}
                        </Typography>
                        <Typography className="preview-text" sx={{ fontSize: '24px' }}>
                            ABCDEFGHIJKLMNOPQRSTUVWXYZ
                        </Typography>
                        <Typography className="preview-text" sx={{ fontSize: '24px' }}>
                            abcdefghijklmnopqrstuvwxyz
                        </Typography>
                        <Typography className="preview-text" sx={{ fontSize: '24px' }}>
                            0123456789
                        </Typography>
                    </Box>
                );
            default:
                return (
                    <Typography>
                        {translate('assets.preview.unsupportedType')}
                    </Typography>
                );
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
                {translate('assets.preview.title', { name: asset.name })}
            </DialogTitle>
            <DialogContent>
                <Box sx={{ mt: 2 }}>
                    {renderPreview()}
                </Box>
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose}>
                    {translate('ra.action.close')}
                </Button>
            </DialogActions>
        </Dialog>
    );
}; 