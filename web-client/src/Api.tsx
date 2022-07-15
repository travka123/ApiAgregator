import Account from "./types/Account";

const connection = "https://localhost:7050"

interface OkResult<T> {
    success: true;
    data: T;
}

interface ErrResult<E> {
    success: false;
    message?: string;
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