import { DefaultButton, Dialog, DialogFooter, Dropdown, IconButton, PrimaryButton, TextField, Toggle } from "@fluentui/react";
import { useBoolean } from "@fluentui/react-hooks";
import { useEffect, useState } from "react";
import { Controller } from "react-hook-form";
import { getReleaseVersions } from "../api";
import { usePackagesSummaryValue } from "../state";

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

export const TextController = ({ control, label, name, type = 'text', required, onRenderSuffix, validate }) => {
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
        value={value}
        required={required}
        onChange={(ev, value) => onChange(value)}
        onBlur={onBlur}
        componentRef={ref}
        errorMessage={error?.message}
        onRenderSuffix={onRenderSuffix}/>
    )}
  />
}

export const NullableTextController = ({ control, label, name, type = 'text' }) => {
  return <Controller
    name={name}
    control={control}
    rules={{setValueAs: v => v === '' || v === undefined ? null : v}}
    render={({
      field: { onChange, onBlur, value, name, ref },
    }) => (
      <TextField
        type={type}
        label={label}
        name={name}
        value={value === null ? '' : value}
        onChange={(ev, value) => onChange(value)}
        onBlur={onBlur}
        componentRef={ref}/>
    )}
  />
}

export const DropdownController = ({ control, label, name, options }) => {
  return <Controller
    name={name}
    control={control}
    render={({
      field: { onChange, onBlur, value, name, ref },
    }) => (
      <Dropdown
        label={label}
        name={name}
        options={options}
        selectedKey={value}
        onChange={(ev, value) => onChange(value.key)}
        onBlur={onBlur}
        componentRef={ref} />
    )}
  />
}

export const ToggleController = ({ control, label, name, onText, offText }) => {
  return <Controller
    name={name}
    control={control}
    render={({
      field: { onChange, onBlur, value, name, ref },
    }) => (
      <Toggle
        label={label}
        onText={onText}
        offText={offText}
        name={name}
        checked={value}
        onChange={(ev, value) => onChange(value)}
        onBlur={onBlur}
        componentRef={ref} />
    )}
  />
}

export const IntegerController = ({ control, label, name }) => {
  return <Controller
    name={name}
    control={control}
    rules={{ required: rules.required, pattern: rules.integer, setValueAs: v => parseInt(v) }}
    render={({
      field: { onChange, onBlur, value, name, ref },
      fieldState: { error }
    }) => (
      <TextField
        label={label}
        name={name}
        value={value}
        onChange={(ev, value) => onChange(value)}
        onBlur={onBlur}
        componentRef={ref}
        errorMessage={error?.message} />
    )}
  />
}

export const PercentController = ({ control, label, name }) => {
  return <Controller
    name={name}
    control={control}
    rules={{ required: rules.required, pattern: rules.integer, max: rules.percentMax, min: rules.percentMin, setValueAs: v => parseInt(v) }}
    render={({
      field: { onChange, onBlur, value, name, ref },
      fieldState: { error }
    }) => (
      <TextField
        label={label}
        name={name}
        value={value}
        onChange={(ev, value) => onChange(value)}
        onBlur={onBlur}
        componentRef={ref}
        errorMessage={error?.message} />
    )}
  />
}

export const HexController = ({ control, label, name }) => {
  return <Controller
    name={name}
    control={control}
    rules={{ required: rules.required, pattern: rules.hex, setValueAs: v => v.constructor === String ? parseInt(v, 16) : v }}
    render={({
      field: { onChange, onBlur, value, name, ref },
      fieldState: { error }
    }) => (
      <TextField
        label={label}
        name={name}
        value={value?.toString(16)}
        onChange={(ev, value) => onChange(value)}
        onBlur={onBlur}
        componentRef={ref}
        errorMessage={error?.message}
        prefix="#" />
    )}
  />
}

export const TimeController = ({ control, label, name, onRenderSuffix }) => {
  return <Controller
    name={name}
    control={control}
    rules={{ required: rules.required, pattern: rules.time }}
    render={({
      field: { onChange, onBlur, value, name, ref },
      fieldState: { error }
    }) => (
      <TextField
        label={label}
        name={name}
        value={value}
        onChange={(ev, value) => onChange(value)}
        componentRef={ref}
        errorMessage={error?.message}
        onRenderSuffix={onRenderSuffix} />
    )}
  />
}

export const FlagController = ({ control, labels, name }) => {
  return <Controller
    name={name}
    control={control}
    rules={{ required: rules.required }}
    render={({
      field: { onChange, onBlur, value, name, ref },
    }) => (
      <>
        {labels.map((label, index) =>
          <Toggle
            key={label}
            label={label}
            name={name}
            checked={value & (1 << index)}
            onChange={(ev, checked) => onChange(checked ? value | (1 << index) : value & ~(1 << index))}
            onBlur={onBlur}
            componentRef={ref}
            prefix="#" />
        )}
      </>
    )}
  />
}

export const DefaultValueController = ({ control, name }) => {
  return <Controller
    name={name}
    control={control}
    render={() => <></>}
  />
}

export const PhysicalPathController = ({ control, name, label }) => {
  const packagesSummary = usePackagesSummaryValue();
  const [hideDialog, { toggle }] = useBoolean(true);
  const [path, setPath] = useState(packagesSummary.locationRoot);
  const [packageName, setPackage] = useState(null);
  const [version, setVersion] = useState(null);
  const packageOptions = packagesSummary.packages.map(name => ({ key: name, text: name }));
  const [versionOptions, setVersionOptions] = useState([]);
  useEffect(() => {
    (async () => {
      const versions = await getReleaseVersions(packageName);
      setVersionOptions(versions.map(version => ({ key: version, text: version })));
    })()
  }, [packageName])
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
          value={value}
          required={true}
          onChange={(ev, value) => onChange(value)}
          onBlur={onBlur}
          componentRef={ref}
          onRenderSuffix={() => <IconButton iconProps={{ iconName: 'edit' }} onClick={toggle}></IconButton>} />
        <Dialog
          hidden={hideDialog}
          onDismiss={toggle}
          title="Physical Path"
        >
          <TextField label="Directory Path" value={path} onChange={(ev, value) => setPath(value)} />
          <Dropdown label="Package" options={packageOptions} onChange={(ev, value) => setPackage(value.key)} />
          <Dropdown label="Version" options={versionOptions} onChange={(ev, value) => setVersion(value.key)} />
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