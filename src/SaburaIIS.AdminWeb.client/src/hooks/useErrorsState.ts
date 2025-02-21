import { Dispatch, SetStateAction, useMemo, useState } from "react";
import { strings } from "../helper";

export default function useErrorsState(...errors: (string|null|undefined)[]): [value: string[], set:Dispatch<SetStateAction<string[]>>] {
const [state, setState] = useState<string[]>([]);
const value = useMemo(() => strings(...errors, ...state), [errors, state]);
return [value, setState];
}