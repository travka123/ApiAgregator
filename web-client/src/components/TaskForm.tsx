import React, { FormEventHandler, useContext, useEffect, useState } from "react";
import { useFieldArray, useForm } from "react-hook-form";
import { ApiForm, getApiForm } from "../Api";
import Account from "../types/Account";
import Task from "../types/Task";
import { AccountContext } from "./Authentication";

interface TaskFormProps {
    title: string;
    submitBtnText: string;
    task?: Task;
    apis: string[];
    onSubmit:  (task: Task) => Promise<string|void>;
    onClose?: () => void;
}

const TaskForm: React.FC<TaskFormProps> = ({ task, submitBtnText, title, apis, onSubmit, onClose }) => {

    const { register, watch, unregister, getValues, setError, formState: {errors}, clearErrors } = useForm<Task>({ ...(task ? { defaultValues: task } : {}) });

    const apiName = watch("apiName");

    const [apiForm, setApiForm] = useState<ApiForm>({});

    const account = useContext<Account>(AccountContext);

    useEffect(() => {

        for (const key of Object.keys(apiForm)) {
            unregister(`parameters.${key}`);
        }

        (async () => {
            const form = await getApiForm(account.jwt, apiName);
            if (form.success) {
                console.log(form.data);
                setApiForm(form.data);
            }
            else {
                setApiForm({});
            }
        })();

    }, [apiName])

    const [failed, setFailed] = useState<boolean>(false);

    const submitHandler = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        clearErrors();

        let task = getValues();

        if (!task.name) {
            setError("name", {type: "required"})
            return;
        }

        let keys = Object.keys(apiForm)

        for (const key of Object.keys(task.parameters)) {
            if (!keys.includes(key)) {
                delete task.parameters[key];
            }
        }

        const error = await onSubmit(task);

        if (!error) {
            if (onClose) onClose();
        }
        else {
            if (error === "invalid cron expression") {
                setError("expression", {type: "invalid"});
            }
            setFailed(true);
        }
    }

    return (
        <div className="TaskForm">
            <h2>{title}</h2>
            <form onSubmit={submitHandler}>

                <div className="mb-3">
                    <label className="form-label">Name</label>
                    <input {...register("name")} className="form-control" />
                    {errors?.name?.type ? <div className="text-danger">required</div> : null}
                </div>

                <div className="mb-3">
                    <label className="form-label">Description</label>
                    <input {...register("description")} type="text" className="form-control" />
                </div>

                <div className="mb-3">
                    <label className="form-label">Cron expression (* * * * *)</label>
                    <input {...register("expression")} type="text" className="form-control" />
                    {errors?.expression?.type ? <div className="text-danger">invalid expression <a href="https://crontab.guru/">help</a></div> : null}
                </div>

                <div className="mb-3">
                    <div>
                        <label className="form-label">Api</label>
                    </div>
                    <select className="form-select" {...register("apiName", { required: true })}>
                        {apis.map(api => <option key={api}>{api}</option>)}
                    </select>
                </div>

                {
                    Object.entries(apiForm).map(([name, info]) =>
                        <div className="mb-3" key={name}>
                            <label className="form-label">{name}</label>
                            {
                                info.type === "select" ?
                                <select className="form-select" {...register(`parameters.${name}`)}>
                                    {info.values.map(opt => <option key={opt}>{opt}</option>)}
                                </select> :
                                info.type === "text" ?
                                <div className="mb-3">
                                    <input {...register(`parameters.${name}`)} className="form-control" />
                                </div> :
                                null
                            }
                        </div>
                    )
                }

                <div className="mb-3">
                    <button className="btn btn-primary me-2" type="submit">{submitBtnText}</button>
                </div>

                <div>
                    {failed ? <h4 className="text-danger">invalid task</h4> : null}
                </div>

            </form>
        </div>
    );
}

export default TaskForm;