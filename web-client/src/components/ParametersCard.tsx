import React from "react";
import Modal from "react-modal";

interface ParametersModalProps {
    parameters: object;
}

const ParametersCard: React.FC<ParametersModalProps> = ({parameters} : ParametersModalProps) => {

    const parametersToNodes = (parameters: object, func: (key: string, value: string, index: number) => React.ReactNode): React.ReactNode[] => {
        let result: React.ReactNode[] = [];
        let counter = 0;
        for (const [key, value] of Object.entries(parameters)) {
            result.push(func(key, value, counter));
            counter++;
        }
        return result;
    } 

    return (
        <div className="ParametersModal" style={{'display': 'inline'}}>      
            {parametersToNodes(parameters, (key, val, i) => <div key={i}>{key}: {val}</div>)}    
        </div>
    );
}

export default ParametersCard;