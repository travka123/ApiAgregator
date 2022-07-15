import React, { createContext, useEffect, useState } from "react"
import Account from "../types/Account";

export const AccountContext = createContext<Account>({ jwt: "", login: "", emailConfirmed: false, email: "", isAdmin: false });

export interface SetAccount {
    (account: Account | null): void;
}

export const SetAccountContext = createContext<SetAccount>(() => { throw new Error("default SetAccountContext call") })

interface AuthenticationProps {
    redirect: React.ReactNode;
    children?: React.ReactNode;
}

export const Authentication: React.FC<AuthenticationProps> = ({ redirect, children }) => {

    const [account, setAccount] = useState<Account | null>(null);

    const [needRedirect, setNeedRedirect] = useState<boolean>(false);

    useEffect(() => {
        const authData = localStorage.getItem("authData");

        if (authData !== null) {
            setAccount(JSON.parse(authData));
        }
        else {
            setNeedRedirect(true);
        }
    }, []);

    const setAccountHandler = (account: Account | null) => {
        if (account === null) {
            localStorage.removeItem("authData");
            setAccount(null);
            setNeedRedirect(true);
        }
        else {
            localStorage.setItem("authData", JSON.stringify(account));
            setAccount(account);
        }
    }

    return (
        <div className="Authentication">
            {account === null ?
                <div>
                    {needRedirect ?
                        <SetAccountContext.Provider value={setAccountHandler}>
                            {redirect}
                        </SetAccountContext.Provider> :
                        null}
                </div> :
                <AccountContext.Provider value={account}>
                    <SetAccountContext.Provider value={setAccountHandler}>
                        {children}
                    </SetAccountContext.Provider>
                </AccountContext.Provider>}
        </div>
    );
}