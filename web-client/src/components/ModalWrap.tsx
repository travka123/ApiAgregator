import React, { useState } from "react"
import Modal from "react-modal";

interface ModalProps {
    openBtn: React.ReactNode;
    content: React.ReactNode;
}

const ModalWrap: React.FC<ModalProps> = ({openBtn, content} : ModalProps) => {

    const customStyles = {
        content: {
            top: '50%',
            left: '50%',
            right: 'auto',
            bottom: 'auto',
            marginRight: '-50%',
            transform: 'translate(-50%, -50%)',
        },
    };

    const [isOpen, setIsOpen] = useState<boolean>(false);

    const open = (e: React.MouseEvent) => {
        e.preventDefault();
        setIsOpen(true);
    }

    const close = () => {
        setIsOpen(false);
    }

    return (
        <div className="Modal me-2" style={{'display': 'inline'}}>
            {React.cloneElement(openBtn as React.ReactElement<any>, {onClick: open })}
            <Modal isOpen={isOpen} onRequestClose={() => setIsOpen(false)} style={customStyles}>
                {React.cloneElement(content as React.ReactElement<any>, {onClose: close})}
            </Modal>
        </div>
    );
}

export default ModalWrap;