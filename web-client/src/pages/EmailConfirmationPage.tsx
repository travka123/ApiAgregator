import React, { useContext, useEffect, useState } from "react"
import { requestEmailConfirmation } from "../Api";
import { AccountContext } from "../components/Authentication";
import Account from "../types/Account";

const EmailConfirmationPage: React.FC = () => {

    const account = useContext<Account>(AccountContext);

    const [disabled, setDisabled] = useState<boolean>(false);

    const sendAgainHandler = (e: React.MouseEvent<HTMLElement>) => {
        e.preventDefault();
        if (!disabled)
        {
            setDisabled(true);
            setTimeout(() => setDisabled(false), 25000);
            requestEmailConfirmation(account.jwt);
        }
    }

    return (
        <div className="EmailConfirmationPage">
            Welcome, {account.login}, account confirmation is required. Please check your email ({account.email}) for the confirmation link. &nbsp;
            <button onClick={sendAgainHandler} disabled={disabled}>Send again.</button>
        </div>
    );
}

export default EmailConfirmationPage;