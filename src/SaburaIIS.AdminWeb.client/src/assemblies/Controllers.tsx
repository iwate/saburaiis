import { DefaultButton, Dialog, DialogFooter, Dropdown, IconButton, IDropdownOption, IDropdownProps, ITextFieldProps, IToggleProps, PrimaryButton, TextField, Toggle } from "@fluentui/react";
import { useBoolean } from "@fluentui/react-hooks";
import { useState } from "react";
import { Control, Controller, FieldPath, FieldValues, RegisterOptions } from "react-hook-form";
import usePackagesSummaryState from "../hooks/usePackagesSummaryState";
import useReleaseListState from "../hooks/useReleaseListState";

const transform = {
  nullable: {
    input: (value: string | undefined | null) => value === undefined || value === null ? '' : value,
    output: (value: string | undefined) => value === '' || value === undefined ? null : value,
  },
  integer: {
    input: (value: string | undefined | null) => value === undefined || value === null ? '' : value,
    output: (value: string | undefined) => parseInt(value ?? '')
  },
  hex: {
    input: (value: string | undefined | null) => value === '' || value === undefined || value === null ? '' : parseInt(value, 16).toString(),
    output: (value: string | undefined) => value === '' || value === undefined ? null : value
  },
  flag: {
    input: (value: number | string | undefined | null, index: number) => {
      const int = typeof value === 'number' ? value
                : value === '' || value === undefined || value === null ? 0
                : parseInt(value);

      return (int & (1 << index)) != 0
    },
    output: (value: number | string | undefined | null, index: number, checked: boolean) => {
      const int = typeof value === 'number' ? value
                : value === '' || value === undefined || value === null ? 0
                : parseInt(value);
      return checked ? int | (1 << index) : int & ~(1 << index)
    }      
  }
}

const rules = {
  required: {
    value: true,
    message: 'This field is requried.'
  },
  integer: {
    value: /^\d+$/,
    message: 'You must input positive integer value (ex. 1000).'
  },
  hex: {
    value: /^[a-f|A-F|0-9]{8}$/,
    message: 'You must input hex integer value (ex. ffffffff).'
  },
  time: {
    value: /^(\d+\.|)\d\d:\d\d:\d\d*$/,
    message: 'You must input time value as (d.)hh:mm.ss (ex. 5min = 00:05:00, 30days = 30.00:00:00)'
  },
  percentMax: {
    value: 100,
    message: 'You must input a value lesser than or equal to 100.'
  },
  percentMin: {
    value: 0,
    message: 'You must input a value greater than or equal to 0'
  }
}

type TextControllerProps<T extends FieldValues, E extends FieldPath<T>> = {
  control: Control<T>
  label?: string
  name: E
  type?: ITextFieldProps['type']
  required?: ITextFieldProps['required']
  readOnly?: ITextFieldProps['readOnly']
  validate?: RegisterOptions<T, E>['validate']
  onRenderSuffix?: ITextFieldProps['onRenderSuffix']
}
export function TextController<T extends FieldValues, E extends FieldPath<T>>({ 
  control, label, name, type = 'text', required, readOnly, onRenderSuffix, validate 
}: TextControllerProps<T, E>) {
  return <Controller
    name={name}
    control={control}
    rules={required ? { required: rules.required, validate } : undefined}
    render={({
      field: { onChange, onBlur, value, name, ref },
      fieldState: { error }
    }) => (
      <TextField
        type={type}
        label={label}
        name={name}
        value={value||''}
        required={required}
        readOnly={readOnly}
        onChange={(_, value) => onChange(value)}
        onBlur={onBlur}
        componentRef={ref}
        errorMessage={error?.message}
        onRenderSuffix={onRenderSuffix}/>
    )}
  />
}

type NullableTextControllerProps<T extends FieldValues, E extends FieldPath<T>> = Pick<TextControllerProps<T, E>, 'control'|'label'|'name'|'type'>
export function NullableTextController<T extends FieldValues, E extends FieldPath<T>>({
  control, label, name, type = 'text' 
}: NullableTextControllerProps<T, E>){
  return <Controller
    name={name}
    control={control}
    render={({
      field: { onChange, onBlur, value, name, ref },
    }) => (
      <TextField
        type={type}
        label={label}
        name={name}
        value={transform.nullable.input(value)}
        onChange={(_, value) => onChange(transform.nullable.output(value))}
        onBlur={onBlur}
        componentRef={ref}/>
    )}
  />
}

type DropdownControllerProps<T extends FieldValues, E extends FieldPath<T>> = {
  control: Control<T>
  label: string
  name: E
  options: IDropdownProps['options']
}
export function DropdownController<T extends FieldValues, E extends FieldPath<T>>({ 
  control, label, name, options
}: DropdownControllerProps<T, E>) {
  return <Controller
    name={name}
    control={control}
    render={({
      field: { onChange, onBlur, value, ref },
    }) => (
      <Dropdown
        label={label}
        options={options}
        selectedKey={value}
        onChange={(_, value) => onChange(value?.key)}
        onBlur={onBlur}
        componentRef={ref} />
    )}
  />
}

type ToggleControllerProps<T extends FieldValues, E extends FieldPath<T>> = {
  control: Control<T>
  label: string
  name: E
  onText: IToggleProps['onText']
  offText: IToggleProps['offText']
}
export function ToggleController<T extends FieldValues, E extends FieldPath<T>>({ 
  control, label, name, onText, offText
}: ToggleControllerProps<T, E>) {
  return <Controller
    name={name}
    control={control}
    render={({
      field: { onChange, onBlur, value, ref },
    }) => (
      <Toggle
        label={label}
        onText={onText}
        offText={offText}
        checked={value}
        onChange={(_, value) => onChange(value)}
        onBlur={onBlur}
        componentRef={ref} />
    )}
  />
}

type IntegerControllerProps<T extends FieldValues, E extends FieldPath<T>> = {
  control: Control<T>
  label: string
  name: E
}
export function IntegerController<T extends FieldValues, E extends FieldPath<T>>({ 
  control, label, name
}: IntegerControllerProps<T, E>) {
  return <Controller
    name={name}
    control={control}
    rules={{ required: rules.required, pattern: rules.integer }}
    render={({
      field: { onChange, onBlur, value, name, ref },
      fieldState: { error }
    }) => (
      <TextField
        label={label}
        name={name}
        value={transform.integer.input(value)}
        onChange={(_, value) => onChange(transform.integer.output(value))}
        onBlur={onBlur}
        componentRef={ref}
        errorMessage={error?.message} />
    )}
  />
}

type PercentControllerProps<T extends FieldValues, E extends FieldPath<T>> = {
  control: Control<T>
  label: string
  name: E
}
export function PercentController<T extends FieldValues, E extends FieldPath<T>>({ 
  control, label, name
}: PercentControllerProps<T, E>) {
  return <Controller
    name={name}
    control={control}
    rules={{ required: rules.required, pattern: rules.integer, max: rules.percentMax, min: rules.percentMin }}
    render={({
      field: { onChange, onBlur, value, name, ref },
      fieldState: { error }
    }) => (
      <TextField
        label={label}
        name={name}
        value={transform.integer.input(value)}
        onChange={(_, value) => onChange(transform.integer.output(value))}
        onBlur={onBlur}
        componentRef={ref}
        errorMessage={error?.message} />
    )}
  />
}

type HexControllerProps<T extends FieldValues, E extends FieldPath<T>> = {
  control: Control<T>
  label: string
  name: E
}
export function HexController<T extends FieldValues, E extends FieldPath<T>>({ 
  control, label, name 
}: HexControllerProps<T, E>) {
  return <Controller
    name={name}
    control={control}
    rules={{ required: rules.required, pattern: rules.hex }}
    render={({
      field: { onChange, onBlur, value, name, ref },
      fieldState: { error }
    }) => (
      <TextField
        label={label}
        name={name}
        value={transform.hex.input(value)}
        onChange={(_, value) => onChange(transform.hex.output(value))}
        onBlur={onBlur}
        componentRef={ref}
        errorMessage={error?.message}
        prefix="#" />
    )}
  />
}

type TimeControllerProps<T extends FieldValues, E extends FieldPath<T>> = {
  control: Control<T>
  label: string
  name: E
  onRenderSuffix?: ITextFieldProps['onRenderSuffix']
}
export function TimeController<T extends FieldValues, E extends FieldPath<T>>({ 
  control, label, name, onRenderSuffix
}: TimeControllerProps<T, E>) {
  return <Controller
    name={name}
    control={control}
    rules={{ required: rules.required, pattern: rules.time }}
    render={({
      field: { onChange, value, name, ref },
      fieldState: { error }
    }) => (
      <TextField
        label={label}
        name={name}
        value={value||''}
        onChange={(_, value) => onChange(value)}
        componentRef={ref}
        errorMessage={error?.message}
        onRenderSuffix={onRenderSuffix} />
    )}
  />
}

type FlagControllerProps<T extends FieldValues, E extends FieldPath<T>> = {
  control: Control<T>
  labels: string[]
  name: E
}
export function FlagController<T extends FieldValues, E extends FieldPath<T>>({ 
  control, labels, name
}: FlagControllerProps<T, E>) {
  return <Controller
    name={name}
    control={control}
    rules={{ required: rules.required }}
    render={({
      field: { onChange, onBlur, value, ref },
    }) => (
      <>
        {labels.map((label, index) =>
          <Toggle
            key={label}
            label={label}
            checked={transform.flag.input(value, index)}
            onChange={(_, checked) => onChange(transform.flag.output(value, index, checked ?? false))}
            onBlur={onBlur}
            componentRef={ref}
            prefix="#" />
        )}
      </>
    )}
  />
}

type DefaultValueControllerProps<T extends FieldValues, E extends FieldPath<T>> = {
  control: Control<T>
  name: E
}
export function DefaultValueController<T extends FieldValues, E extends FieldPath<T>>({ 
  control, name
}: DefaultValueControllerProps<T, E>) {
  return <Controller
    name={name}
    control={control}
    render={() => <></>}
  />
}

type PhysicalPathControllerProps<T extends FieldValues, E extends FieldPath<T>> = {
  control: Control<T>
  label: string
  name: E
}
export function PhysicalPathController<T extends FieldValues, E extends FieldPath<T>>({ 
  control, name, label 
}: PhysicalPathControllerProps<T, E>) {
  const [packagesSummary] = usePackagesSummaryState();
  const [hideDialog, { toggle }] = useBoolean(true);
  const [path, setPath] = useState(packagesSummary?.locationRoot);
  const [packageName, setPackage] = useState<string|null>(null);
  const [versions] = useReleaseListState(packageName);
  const [version, setVersion] = useState<string|null>(null);
  const packageOptions:IDropdownOption[] = packagesSummary?.packages.map(name => ({ key: name, text: name })) ?? [];
  const versionOptions:IDropdownOption[] = versions?.map(version => ({ key: version, text: version })) ?? [];
  return <Controller
    name={name}
    control={control}
    rules={{ required: rules.required }}
    render={({
      field: { onChange, onBlur, value, name, ref },
    }) => (<>
        <TextField
          label={label}
          name={name}
          value={value||''}
          required={true}
          readOnly={packagesSummary===undefined}
          onChange={(_, value) => onChange(value)}
          onBlur={onBlur}
          componentRef={ref}
          onRenderSuffix={() => <IconButton iconProps={{ iconName: 'edit' }} onClick={toggle}></IconButton>} />
        <Dialog
          hidden={hideDialog}
          onDismiss={toggle}
          dialogContentProps={{
            title: 'Physical Path'
          }}
        >
          <TextField label="Directory Path" value={path||''} onChange={(_, value) => setPath(value)} />
          <Dropdown label="Package" options={packageOptions} onChange={(_, value) => setPackage(String(value?.key))} />
          <Dropdown label="Version" options={versionOptions} onChange={(_, value) => setVersion(String(value?.key))} />
          <DialogFooter>
            <PrimaryButton
              disabled={packageName == null || version == null}
              onClick={() => {
                onChange([path, packageName, version].join('\\').replace(/\\\\/g, '\\'));
                toggle();
              }}>Update</PrimaryButton>
            <DefaultButton onClick={toggle}>Cancel</DefaultButton>
          </DialogFooter>
        </Dialog>
      </>)}
  />
}