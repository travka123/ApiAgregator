import React from "react";
import Task from "../types/Task";

interface StTaskCardProps {
    task: Task;
    extra?: React.ReactNode[]; 
}

const printParams = (params: object): string => {
    let result = "";
    for (const [key, value] of Object.entries(params)) {
        result += `${key}: ${value}`;
    }
    return result;
}

const printTime = (date: Date): string => {
    return date.toLocaleString('en-GB', { weekday:"long", year:"numeric", month:"short", day:"numeric", hour:"numeric", minute:"numeric"});
}

const StTaskCard: React.FC<StTaskCardProps> = ({ task, extra }) => {
    return (
        <div className="StTaskCard mb-2">
            <div className="card">
                <div className="card-body">

                    <h5 className="card-title">{task.name}</h5>

                    <h6 className="card-subtitle text-muted mb-0">{task.apiName} ({task.expression})</h6>

                    <p className="card-text mt-1 mb-1">{task.description}</p>

                    {extra ? extra.map((e, i) => React.cloneElement(e as React.ReactElement<any>, {key: i})) : null}

                    <p className="card-text">
                        <small className="text-muted">
                            {`Last fire: ${task.lastFire !== null ? printTime(task.lastFire) : 'never'}`}
                        </small>
                    </p>

                </div>
            </div>
        </div>
    );
}

export default StTaskCard;