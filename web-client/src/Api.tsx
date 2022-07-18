import Account from "./types/Account";
import Task from "./types/Task";
import { UserStat } from "./types/UserStat";

const connection = "https://localhost:7050"

interface OkResult<T> {
    success: true;
    data: T;
}

interface ErrResult<E> {
    success: false;
    message?: E;
}

type Result<T, E> = OkResult<T> | ErrResult<E>; 

export const signinByUsername = async (login: string, password: string): Promise<Result<Account, null>> => {
    const response = await fetch(`${connection}/guest/signinbyusername`, {
        method: "POST",
        mode: "cors",
        body: JSON.stringify({
            username: login,
            password: btoa(password)
        }),
        headers: {
            'Content-Type': 'application/json'
        }
    });
    if (response.status === 200) {
        return { success: true, data: await response.json() };
    }
    else {
        return { success: false };
    }
}

export const checkEmailConfirmation = async (jwt: string): Promise<Result<Account, null>> => {
    const response = await fetch(`${connection}/emailvalidation/checkconfirmation`, {
        method: "GET",
        mode: "cors",
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${jwt}`
        }
    });
    if (response.status === 200) {
        return { success: true, data: await response.json() };
    }
    else {
        return { success: false };
    }
}

export const requestEmailConfirmation = async (jwt: string): Promise<Result<null, null>> => {
    const response = await fetch(`${connection}/emailvalidation/requestconfirmation`, {
        method: "GET",
        mode: "cors",
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${jwt}`
        }
    });
    if (response.status === 200) {
        return { success: true, data: null };
    }
    else {
        return { success: false };
    }
}

export const confirmEmail = async (jwt:string, token: string): Promise<Result<null, null>> => {
    const response = await fetch(`${connection}/emailvalidation/confirm`, {
        method: "POST",
        mode: "cors",
        body: JSON.stringify({
            token: token
        }),
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${jwt}`
        }
    });
    if (response.status === 200) {
        return { success: true, data: null };
    }
    else {
        return { success: false };
    }
}

export const signup = async (login: string, email: string, password: string): Promise<Result<null, "username already in use"|null>> => {
    const response = await fetch(`${connection}/guest/signup`, {
        method: "POST",
        mode: "cors",
        body: JSON.stringify({
            username: login,
            password: btoa(password),
            email: email
        }),
        headers: {
            'Content-Type': 'application/json'
        }
    });
    if (response.status === 200) {
        return { success: true, data: null };
    }
    else if (response.status === 409) {
        return { success: false, message: "username already in use" };
    }
    else {
        return { success: false };
    }
}

export const getTasks = async (jwt: string): Promise<Result<Task[], null>> => {
    const response = await fetch(`${connection}/user/tasks`, {
        method: "GET",
        mode: "cors",
        headers: {
            'Authorization': `Bearer ${jwt}`
        }
    });
    if (response.status === 200) {
        const data = (await response.json()).map((t:any) => { return {...t, lastFire: t.lastFire ? new Date(t.lastFire) : null}});
        return { success: true, data: data }
    }
    else {
        return { success: false };
    }
}

export const getApis = async (jwt: string): Promise<Result<string[], null>> => {
    const response = await fetch(`${connection}/user/apis`, {
        method: "GET",
        mode: "cors",
        headers: {
            'Authorization': `Bearer ${jwt}`
        }
    });
    if (response.status === 200) {
        return { success: true, data: await response.json() }
    }
    else {
        return { success: false };
    }
}

interface FormSelectItem {
    type: "select";
    values: string[];
}

interface FormTextItem {
    type: "text";
}

export type ApiForm = Record<string, FormSelectItem|FormTextItem>;

export const getApiForm = async (jwt: string, api: string): Promise<Result<ApiForm, null>> => {
    const response = await fetch(`${connection}/user/apis/${api}`, {
        method: "GET",
        mode: "cors",
        headers: {
            'Authorization': `Bearer ${jwt}`
        }
    });
    if (response.status === 200) {
        return { success: true, data: await response.json() }
    }
    else {
        return { success: false };
    }
}

export const updateTask = async (jwt: string, task:Task): Promise<Result<Task, string>> => {
    const response = await fetch(`${connection}/user/tasks/${task.id}`, {
        method: "PUT",
        mode: "cors",
        headers: {
            'Authorization': `Bearer ${jwt}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(task)
    });
    if (response.status === 200) {
        const data = await response.json();
        data.lastFire = data.lastFire ? new Date(data.lastFire) : null;
        return { success: true, data: data }
    }
    else {
        if (await response.text() === "invalid cron expression") {
            return { success: false, message: "invalid cron expression" };
        }
        return { success: false };
    }
}

export const addTask = async (jwt: string, task:Task): Promise<Result<Task, string>> => {
    const response = await fetch(`${connection}/user/tasks`, {
        method: "POST",
        mode: "cors",
        headers: {
            'Authorization': `Bearer ${jwt}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(task)
    });
    if (response.status === 200) {
        const data = await response.json();
        data.lastFire = data.lastFire ? new Date(data.lastFire) : null;
        return { success: true, data: data }
    }
    else {
        if (await response.text() === "invalid cron expression") {
            return { success: false, message: "invalid cron expression" };
        }
        return { success: false };
    }
}

export const deleteTask = async (jwt: string, taskId: number): Promise<Result<null, null>> => {
    const response = await fetch(`${connection}/user/tasks/${taskId}`, {
        method: "DELETE",
        mode: "cors",
        headers: {
            'Authorization': `Bearer ${jwt}`
        }
    });
    if (response.status === 200) {
        return { success: true, data: null }
    }
    else {
        return { success: false };
    }
}

export const getUserStats = async (jwt: string): Promise<Result<UserStat[], null>> => {
    const response = await fetch(`${connection}/admin/statistics`, {
        method: "GET",
        mode: "cors",
        headers: {
            'Authorization': `Bearer ${jwt}`
        }
    });
    if (response.status === 200) {
        return { success: true, data: await response.json() }
    }
    else {
        return { success: false };
    }
}
