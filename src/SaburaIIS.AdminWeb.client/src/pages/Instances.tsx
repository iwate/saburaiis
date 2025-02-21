import { Stack } from "@fluentui/react";
import { Outlet } from "react-router";

export default function Instances() {
  return (
    <Stack style={{ width: '100%' }}>
      <Outlet/>
    </Stack>
  )
}