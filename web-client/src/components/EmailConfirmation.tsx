import React, { useContext, useEffect, useState } from "react";
import { checkEmailConfirmation } from "../Api";
import Account from "../types/Account";
import { AccountContext, SetAccount, SetAccountContext } from "./Authentication";

interface EmailConfirmationProps {
    redirect: React.ReactNode;
    children?: React.ReactNode;
}

const EmailConfirmation: React.FC<EmailConfirmationProps> = ({redirect, children}) => {

    const [checked, setChecked] = useState<boolean>(false);

    const account = useContext<Account>(AccountContext);
    const setAccount = useContext<SetAccount>(SetAccountContext);

    useEffect(() => {
        if (!account.emailConfirmed) {
            (async () => {
                const result = await checkEmailConfirmation(account.jwt);
                if (result.success) {
                    setAccount(result.data);
                }
                setChecked(true);
            })();
        }
    }, [account, setAccount]);

    return (
        <div className="EmailConfirmation">
            {account.emailConfirmed ? 
                children :
                checked ?
                    redirect :
                    null}
        </div>
    );
}

export default EmailConfirmation;