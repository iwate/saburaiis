import { createContext, Dispatch, FC, SetStateAction, useState } from "react"
import { IPartition } from "../type"

export type IWorkingContext = {
  value: {
    partitions: { [key: string]: IPartition }
    diffs: { [key: string]: boolean }
  },
  actions: {
    set: Dispatch<SetStateAction<IWorkingContext['value']>>
    setPartition: (partition: IPartition, hasDiff: boolean) => void
  }
}

const defaultValue: IWorkingContext = {
  value: {
    partitions: {},
    diffs: {}
  },
  actions: {
    set: () => {},
    setPartition: () => {}
  }
}

export const WorkingContext = createContext<IWorkingContext>(defaultValue);

export const WorkingContextProvider: FC<{children: JSX.Element}> = ({children}) => {
  const [value, set] = useState<IWorkingContext['value']>(defaultValue.value);
  const actions: IWorkingContext['actions'] = {
    set,
    setPartition(partition, hasDiff) {
      set(value => ({
        partitions: {
          ...value.partitions,
          [partition.name]: { ...partition }
        },
        diffs: {
          ...value.diffs,
          [partition.name]: hasDiff
        }
      }))
    }
  }
  const context = { value, actions }
  return (
    <WorkingContext.Provider value={context}>
      {children}
    </WorkingContext.Provider>
  )
}