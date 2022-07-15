import React, { useCallback, useContext, useEffect } from "react"
import { useNavigate, useParams } from "react-router-dom";
import { checkEmailConfirmation, confirmEmail } from "../Api";
import { AccountContext, SetAccountContext } from "./Authentication";

const EmailConfirmationHandler: React.FC = () => {

    const { token } = useParams();

    const navigate = useNavigate();

    const account = useContext(AccountContext);
    const setAccount = useContext(SetAccountContext);

    useEffect(() => {
        (async () => {
            if (typeof token === "string") {
                const result = await confirmEmail(account.jwt, token);
                if (result.success) {
                    const result = await checkEmailConfirmation(account.jwt);
                    if (result.success) {
                        setAccount(result.data);
                        navigate("/");
                    }
                }
            }
        })();
    }, [])

    return (<div className="EmailConfirmationHandler"></div>);
}

export default EmailConfirmationHandler;