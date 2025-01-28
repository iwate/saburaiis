import { Component } from "react";

type Props = {
    fallback: JSX.Element|string
    children: JSX.Element
}

type State = {
    hasError: boolean
}

export default class ErrorBoundary extends Component<Props, State> {
    constructor(props: Props) {
      super(props);
      this.state = { hasError: false };
    }
  
    static getDerivedStateFromError() {
      return { hasError: true };
    }
  
    componentDidCatch() {
    }
  
    render() {
      if (this.state.hasError) {
        return this.props.fallback;
      }
  
      return this.props.children;
    }
  }