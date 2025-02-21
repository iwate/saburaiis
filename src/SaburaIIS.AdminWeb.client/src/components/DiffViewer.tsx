import * as monaco from 'monaco-editor'
import { Component, createRef } from 'react'

type Props = {
  original: string
  modified: string
}

export default class DiffViewer extends Component<Props> {
  constructor(props: Props) {
    super(props);
  }

  private divRef = createRef<HTMLDivElement>();
  private editorRef = createRef<monaco.editor.IStandaloneDiffEditor>();

  componentDidMount() {
    if (this.divRef.current) {
      // @ts-expect-error ignore
      this.editorRef.current = monaco.editor.createDiffEditor(this.divRef.current, {
        readOnly: true,
        enableSplitViewResizing: true,
        renderSideBySide: true,
        automaticLayout: true,
        useInlineViewWhenSpaceIsLimited: false
      })

      const originalModel = monaco.editor.createModel(this.props.original, 'application/json');
      const modifiedModel = monaco.editor.createModel(this.props.modified, 'application/json');

      this.editorRef.current.setModel({
        original: originalModel,
        modified: modifiedModel,
      });
    }
    else {
      console.error('div is missing');
    }
  }

  componentWillUnmount() {
    if (this.editorRef.current) {
      this.editorRef.current.dispose();
      // @ts-expect-error ignore
      this.editorRef.current = null;
    }
  }

  shouldComponentUpdate(nextProps: Props) {
    if (this.editorRef.current) {
      const model = this.editorRef.current.getModel();
      model?.original.setValue(nextProps.original);
      model?.modified.setValue(nextProps.modified);
    }
    return false;
  }
  

  render() {
    return (
      <div ref={this.divRef} style={{height:'100%'}}></div>
    )
  }
}