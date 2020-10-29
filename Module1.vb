Imports System.Collections.Generic
Imports System.Text
''' <summary>
''' 中英文对齐的处理：等宽字体中文的宽度是英文宽度的两倍
''' 统计最大长度时，把全角字符当作两个半角字符
''' 打印输出时，扣除全角字符多占用的宽度
''' </summary>
Module Module1
    Class FileDirListItem '文件目录列表条目
        Public Property Name As String
        Public Property Type As String
        Public Sub New(ByVal name As String, ByVal type As String) '构造函数
            Me.Name = name
            Me.Type = type
        End Sub
    End Class
    Sub Main(args As String())
        If args.Length > 0 Then
            Console.WriteLine("此命令无参数。")
            Exit Sub
        End If
        Dim listMain As New List(Of FileDirListItem) '声明文件目录列表

        '获取控制台缓冲区大小
        Dim buffH As Integer = Console.BufferHeight
        Dim buffW As Integer = Console.BufferWidth

        '打开当前文件夹并依次把目录、文件添加到列表
        Dim dInfo As New IO.DirectoryInfo(".")
        Dim arrayFile As IO.FileInfo() = dInfo.GetFiles()
        Dim arrayDir As IO.DirectoryInfo() = dInfo.GetDirectories()
        For Each d In arrayDir
            If Not d.Attributes.HasFlag(IO.FileAttributes.Hidden) Then
                listMain.Add(New FileDirListItem(d.Name, "Directory"))
            End If
        Next
        For Each f In arrayFile
            If Not f.Attributes.HasFlag(IO.FileAttributes.Hidden) Then
                listMain.Add(New FileDirListItem(f.Name, "File"))
            End If
        Next

        '获取最大列宽
        Dim colW As Integer = 0
        For Each li In listMain
            If li.Name.Length > colW Then
                colW = li.Name.Length + FullWidthCharCount(li.Name)
            End If
        Next

        '增加间距
        colW += 4

        '总元素个数
        Dim nItem As Integer = listMain.Count

        '列数
        Dim nCol As Integer = buffW / colW

        '行数 注意 vb.net 除法返回值是浮点型，这里需要向下取整
        Dim nRow As Integer
        If nItem Mod nCol <> 0 Then
            nRow = Int(nItem / nCol) + 1
        Else
            nRow = Int(nItem / nCol)
        End If

        '按列输出： 计算索引值，从列表中取出元素
        Dim i As Integer
        For nr = 0 To nRow - 1
            For nc = 0 To nCol - 1
                i = (nc * nRow) + nr
                If i >= listMain.Count Then
                    Exit Sub
                End If
                If listMain(i).Type = "Directory" Then
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.Write(listMain(i).Name.PadRight(colW - FullWidthCharCount(listMain(i).Name)))
                    Console.ResetColor()
                Else
                    Console.ForegroundColor = ConsoleColor.Green
                    Console.Write(listMain(i).Name.PadRight(colW - FullWidthCharCount(listMain(i).Name)))
                    Console.ResetColor()
                End If
            Next
            Console.WriteLine("")
        Next

    End Sub

    ''' <summary>
    ''' 统计字符串中的全角字符，判断依据是字符的字节大小
    ''' </summary>
    Private Function FullWidthCharCount(ByVal s As String) As Integer
        Dim r As Integer = 0
        For Each c In s.ToCharArray()
            If Encoding.Default.GetByteCount(c) > 1 Then '普通ASCII码为一个字节，如果大于一个字节，判定为全角字符
                r += 1
            End If
        Next
        Return r
    End Function
End Module
